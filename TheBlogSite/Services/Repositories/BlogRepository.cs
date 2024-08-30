using Microsoft.EntityFrameworkCore;
using TheBlogSite.Data;
using TheBlogSite.Services.Interfaces;
using TheBlogSite.Models;
using System.Net.Http;
using TheBlogSite.Client.Models;
using Microsoft.EntityFrameworkCore.Internal;
using System.Reflection.Metadata;
using System.Globalization;
using Humanizer;
using System.Text.RegularExpressions;

namespace TheBlogSite.Services.Repository
{
	public class BlogRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : IBlogPostRepository
	{

        #region slugs
        private async Task<string> GenerateSlugAsync(string title, int id)
        {
            if (await ValidateSlugAsync(title, id))
            {
                return Slugify(title); // "my-first-post"
            }
            else
            {
                int i = 2;
                string newTitle = $"{title} {i}"; // "my-first-post-2"
                bool isValid = await ValidateSlugAsync(newTitle, id);

                while (isValid == false)
                {
                    i++;
                    newTitle = $"{title} {i}"; // "my-first-post-3"
                    isValid = await ValidateSlugAsync(newTitle, id);
                }

                return Slugify(newTitle);
            }
        }

        private async Task<bool> ValidateSlugAsync(string title, int blogId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            string newSlug = Slugify(title);

            bool isValid = false;
            if (blogId == 0)
            {
                // this is a new post, so just check if anyone has this slug
                isValid = !await context.BlogPosts.AnyAsync(bp => bp.Slug == newSlug);
            }
            else
            {
                // this is an existing post, so see if any OTHER posts have this slug
                isValid = !await context.BlogPosts.AnyAsync(bp => bp.Slug == newSlug && bp.Id != blogId);
            }

            return isValid;
        }

        private static string Slugify(string title)
        {
            if (string.IsNullOrEmpty(title)) return title;

            title = title.Normalize(System.Text.NormalizationForm.FormD);
            char[] chars = title.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();

            string normalizedTitle = new string(chars).Normalize(System.Text.NormalizationForm.FormC)
                                                      .ToLower()
                                                      .Trim();

            string titleWithoutSymbols = Regex.Replace(normalizedTitle, @"[^A-Za-z0-9\s-]", "");
            string slug = Regex.Replace(titleWithoutSymbols, @"\s+", "-");

            return slug;
        }
        #endregion

        public async Task AddTagsToBlogPostAsync(int blogPostId, IEnumerable<string> tagNames) // related to RemoveTagsFromBlogPostsASync
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();
			TextInfo textInfo = new CultureInfo("en-US").TextInfo;

			//object TextInfo call it textInfo and strings needs to be culturally correct (language syntax)

			BlogPost? blogPost = await context.BlogPosts
											  // no DTO in the repos!!
											  .Include(b => b.Tags)
											  .FirstOrDefaultAsync(b => b.Id == blogPostId);
			//use your linq statement for 1 blogPost 1 instantuate blogpost 
			// using the parameter of blogPostId --> primary key distinct integer 
			//conjoins based on relationship with two tables (using ICollection)
			if (blogPost != null)
			{
				foreach (var tagName in tagNames)
				// var is implicit == implying indivdiual variable tag will be tagNames can use string too
				{
					Tag? existingTag = await context.Tags.FirstOrDefaultAsync(t => t.Name!.Trim().ToLower() == tagName.Trim().ToLower());
					//ienumerable singular item Tag? use a FirstorDefault trim off anything lyimg around that you cannot see
					if (existingTag != null)
					{
						blogPost.Tags.Add(existingTag);
						//blogPost list o ftags that can be added to or removed from 
						// is not ienumerbale it is an icollection 
					}
					else
					{
						string titleCaseTagName = textInfo.ToTitleCase(tagName.Trim());
						//explicit = string need to be captialize and US culture 

						Tag newTag = new Tag() { Name = titleCaseTagName };
						context.Tags.Add(newTag);
						blogPost.Tags.Add(newTag);
					}
				}
				await context.SaveChangesAsync();
			}
		}

		
		public async Task<BlogPost> CreateBlogPostAsync(BlogPost blogPost)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			blogPost.Created = DateTimeOffset.Now;
            //get slug here <---
            blogPost.Slug = await GenerateSlugAsync(blogPost.Title!, blogPost.Id);

            context.Add(blogPost);
			await context.SaveChangesAsync();

			return blogPost;
		}


		public async Task DeleteBlogPostAsync(int blogPostId)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			BlogPost? blogPost = await context.BlogPosts
												.FirstOrDefaultAsync(b => b.Id == blogPostId);
			if (blogPost is not null)
			//need boolean of IsDeleted and IsPublished to do a soft delete 
			{
				blogPost.IsDeleted = true;
				blogPost.IsPublished = false;

				await context.SaveChangesAsync();
			}
		}


		public async Task<BlogPost?> GetBlogPostByIdAsync(int id)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();
			//eager the data from public virtual()
			BlogPost? post = await context.BlogPosts
											.Where(b => b.IsPublished == true && b.IsDeleted == false)
											.Include(b => b.Category)
											.Include(b => b.Tags)
											.Include(b => b.Comments).ThenInclude(c => c.Author)
											.FirstOrDefaultAsync(b => b.Id == id);
			return post;
		}


		public async Task<BlogPost?> GetBlogPostBySlugAsync(string slug)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			BlogPost? post = await context.BlogPosts
											.Where(b => b.IsPublished == true && b.IsDeleted == false)
											.Include(b => b.Category)
											.Include(b => b.Tags)
											.Include(b => b.Comments).ThenInclude(c => c.Author)
											.FirstOrDefaultAsync(b => b.Slug == slug);
			return post;
		}


		public async Task<IEnumerable<BlogPost>> GetDeletedPostsAsync()
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();
			List<BlogPost> posts = await context.BlogPosts
												 .Where(b => b.IsDeleted == true)
												 .OrderByDescending(b => b.Created)
												 .Include(b => b.Category)
												 .Include(b => b.Comments)
												 .ToListAsync();
			return posts;
		}


		public async Task<IEnumerable<BlogPost>> GetDraftPostsAsync()
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			List<BlogPost> posts = await context.BlogPosts
												.Where(b => b.IsPublished == false && b.IsDeleted == false)
												.Include(b => b.Category)
												.Include(b => b.Comments)
												.OrderByDescending(b => b.Created)
												.ToListAsync();
			return posts;
		}

		public async Task<IEnumerable<BlogPost>> GetPostsByCategoryId(int categoryId)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			IEnumerable<BlogPost> posts = await context.BlogPosts
														.Where(b => b.IsPublished == true && b.IsDeleted == false && b.CategoryId == categoryId)
														.Include(b => b.Category)
														.Include(b => b.Comments)
														.OrderByDescending(b => b.Created)
														.ToListAsync();
			return posts;
		}

		public async Task<IEnumerable<BlogPost>> GetPostsByTagIdAsync(int tagId, int page, int pageSize)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();
			
			IEnumerable<BlogPost> posts = await context.BlogPosts
										// use the Linq where statements w/ a boolean 
										.Where(b => b.IsPublished == true && b.IsDeleted == false && b.Tags.Any(t => t.Id == tagId))
										.Include(b =>b.Category)
										.Include(b => b.Tags)
										.Include (b => b.Comments)
										.OrderByDescending(b => b.Created) 
										.ToListAsync();
			return posts;
		}

		public async Task<PagedList<BlogPost>> GetPublishedPostsAsync(int page, int pageSize)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			PagedList<BlogPost> posts = await context.BlogPosts
														.Where(b => b.IsPublished == true && b.IsDeleted == false)
														.Include(b => b.Category)
														.Include(b => b.Comments)
														.OrderByDescending(b => b.Created)
														.ToPagedListAsync(page, pageSize);
			return posts;
		}

		public async Task<Tag?> GetTagByIdAsync(int tagId)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			Tag? tag = await context.Tags
									.FirstOrDefaultAsync(t => t.Id == tagId);
			return tag;
		}

		public async Task<IEnumerable<BlogPost>> GetTopBlogPostsAsync(int count)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			List<BlogPost> blogPosts = await context.BlogPosts
				.Include(c => c.Comments)
				//.OrderByDescending(c => c.Created)
				//.Count())
				//.Take(count)
				.ToListAsync();

			return blogPosts;
		}

		public async Task PublishBlogPostAsync(int blogPostId)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();
			BlogPost? blogPost = await context.BlogPosts
											   .FirstOrDefaultAsync(b => b.Id == blogPostId);

			if (blogPost is not null)
			{
				blogPost.IsDeleted = false;
				blogPost.IsPublished = true;
				await context.SaveChangesAsync();
			}
		}

		public async Task RemoveTagsFromBlogPostAsync(int blogPostId) // related to AddTagsToBlogPostAsync
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			BlogPost? blogPost = await context.BlogPosts
												.Include(b => b.Tags)
												.FirstOrDefaultAsync(b => b.Id == blogPostId);
			if (blogPost is not null)
			{ 
				blogPost.Tags.Clear();
				await context.SaveChangesAsync();
			}
		}

		public async Task RestoreBlogPostAsync(int blogPostId)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();
			BlogPost? blogPost = await context.BlogPosts
												.FirstOrDefaultAsync(b => b.Id == blogPostId);

			if (blogPost is not null)
			{
				blogPost.IsDeleted = false;
				await context.SaveChangesAsync();
			}
		}

		public async Task<PagedList<BlogPost>> SearchBlogPostsAsync(string query, int page, int pageSize)
		{
            // foreign key relationship shows here 
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            string normalizedQuery = query.Trim().ToLower();

            PagedList<BlogPost> results = await context.BlogPosts
                .Where(b => b.IsPublished == true && b.IsDeleted == false)
                .Include(b => b.Category)
                .Include(b => b.Tags) // needs to be in a list of strings w/ any tag name as well as comments (foreign key table)
                .Include(b => b.Comments) // content of comment (direct property of model ask the author F/L Name that live in another table)
                    .ThenInclude(c => c.Author)
                .Where(b => string.IsNullOrWhiteSpace(normalizedQuery)
                        || b.Title!.ToLower().Contains(normalizedQuery)
                        || b.Abstract!.ToLower().Contains(normalizedQuery)
                        || b.Content!.ToLower().Contains(normalizedQuery)
                        || b.Category!.Name!.ToLower().Contains(normalizedQuery)
                        || b.Tags!.Select(t => t.Name!.ToLower()).Any(tagName => tagName.Contains(normalizedQuery))
                        || b.Comments.Any(c => c.Content!.ToLower().Contains(normalizedQuery)
                                         || c.Author!.FirstName!.ToLower().Contains(normalizedQuery)
                                         || c.Author!.LastName!.ToLower().Contains(normalizedQuery))
                    )
                .OrderByDescending(b => b.Created)
                .ToPagedListAsync(page, pageSize);

            return results;
        }

		public async Task UnpublishBlogPostAsync(int blogPostId)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();
			BlogPost? blogPost = await context.BlogPosts
												.FirstOrDefaultAsync(b => b.Id == blogPostId);

			if (blogPost is not null)
			{
				blogPost.IsPublished = false;
				await context.SaveChangesAsync();
			}
		}

		public async Task UpdateBlogPostAsync(BlogPost blogPost)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			ImageUpload? oldImage = null;

			if (blogPost.Image is not null)
			{
				//check for the old image 
				oldImage = await context.Images.FirstOrDefaultAsync(i => i.Id == blogPost.ImageId);

				//fix the foreign key or relink to the contact image 
				blogPost.ImageId = blogPost.Image.Id;
				context.Images.Add(blogPost.Image);
			}
            //check /get slug here
            blogPost.Slug = await GenerateSlugAsync(blogPost.Title!, blogPost.Id);
            blogPost.Updated = DateTimeOffset.UtcNow;

			context.Update(blogPost);
			await context.SaveChangesAsync();

			if (oldImage is not null)
			{
				context.Images.Remove(oldImage);
				await context.SaveChangesAsync();

			}
		}

   //     public async Task<IEnumerable<BlogPost>> SearchBlogPostsAsync(string query)
   //     {
			//// foreign key relationship shows here 
   //         using ApplicationDbContext context = contextFactory.CreateDbContext();

			//string normalizedQuery = query.Trim().ToLower();

			//IEnumerable<BlogPost> results = await context.BlogPosts
			//	.Where(b => b.IsPublished == true && b.IsDeleted == false)
			//	.Include(b => b.Category)
			//	.Include(b => b.Tags) // needs to be in a list of strings w/ any tag name as well as comments (foreign key table)
			//	.Include(b => b.Comments) // content of comment (direct property of model ask the author F/L Name that live in another table)
			//		.ThenInclude(c=> c.Author)
			//	.Where(b => string.IsNullOrWhiteSpace(normalizedQuery)
			//			|| b.Title!.ToLower().Contains(normalizedQuery)
   //                     || b.Abstract!.ToLower().Contains(normalizedQuery)
   //                     || b.Content!.ToLower().Contains(normalizedQuery)
   //                     || b.Category!.Name!.ToLower().Contains(normalizedQuery)
   //                     || b.Tags!.Select(t => t.Name!.ToLower()).Any(tagName => tagName.Contains(normalizedQuery))
   //                     || b.Comments.Any(c => c.Content!.ToLower().Contains(normalizedQuery)
			//							 || c.Author!.FirstName!.ToLower().Contains(normalizedQuery)
			//							 || c.Author!.LastName!.ToLower().Contains(normalizedQuery))
			//		)
			//	.OrderByDescending(b => b.Created)
			//	.ToListAsync();

			//return results;

   //     }
	}

}

