using TheBlogSite.Client.Models;
using TheBlogSite.Client.Services.Interfaces;
using TheBlogSite.Helpers;
using TheBlogSite.Models;
using TheBlogSite.Services.Interfaces;

namespace TheBlogSite.Services
{
    public class BlogPostService : IBlogPostService
    {
        private readonly IBlogPostRepository _repository;

        public BlogPostService(IBlogPostRepository repository)
        {
            _repository = repository;
        }
        public async Task<BlogPostDTO> CreateBlogPostAsync(BlogPostDTO blogPostDTO)
        {
            BlogPost newPost = new BlogPost()
            {
                Title = blogPostDTO.Title,
                Abstract = blogPostDTO.Abstract,
                Content = blogPostDTO.Content,
                IsPublished = blogPostDTO.IsPublished,
                IsDeleted = false,
                CategoryId = blogPostDTO.CategoryId,
                Created = DateTimeOffset.Now
            };

            if (blogPostDTO.ImageUrl?.StartsWith("data:") == true)
            {
                try
                {
                    newPost.Image = UploadHelper.GetImageUpload(blogPostDTO.ImageUrl);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
                newPost = await _repository.CreateBlogPostAsync(newPost);

                IEnumerable<string> tagNames = blogPostDTO.Tags.Select(t => t.Name!);
            //Handle Tags here
            await _repository.AddTagsToBlogPostAsync(newPost.Id, tagNames);

                return newPost.ToDTO();
        }

        public async Task DeleteBlogPostAsync(int blogPostId)
        {
            await _repository.DeleteBlogPostAsync(blogPostId);
        }

        public async Task<BlogPostDTO?> GetBlogPostByIdAsync(int id)
        {
            BlogPost? post = await _repository.GetBlogPostByIdAsync(id);
            return post?.ToDTO();
        }

        public async Task<BlogPostDTO?> GetBlogPostBySlugAsync(string slug)
        {
            BlogPost? post = await _repository.GetBlogPostBySlugAsync(slug);

            BlogPostDTO? dtoPosts = (await _repository.GetBlogPostBySlugAsync(slug))?.ToDTO();
            return dtoPosts;
        }

        public async Task<IEnumerable<BlogPostDTO>> GetDeletedPostsAsync()
        {
            IEnumerable<BlogPost> posts = await _repository.GetDeletedPostsAsync();
            return posts.Select(b => b.ToDTO());
        }

        public async Task<IEnumerable<BlogPostDTO>> GetDraftPostsAsync()
        {
            IEnumerable<BlogPost> posts = await _repository.GetDraftPostsAsync();
            return posts.Select(b => b.ToDTO());
        }

        public async Task<IEnumerable<BlogPostDTO>> GetPostsByCategoryId(int categoryId)
        {
            IEnumerable<BlogPost> posts = await _repository.GetPostsByCategoryId(categoryId);
            IEnumerable<BlogPostDTO> dtos = posts.Select(p => p.ToDTO());

            return dtos;
        }

        public async Task<IEnumerable<BlogPostDTO>> GetPostsByTagIdAsync(int tagId, int page, int pageSize)
        {
            IEnumerable<BlogPost> posts = await _repository.GetPostsByTagIdAsync(tagId,page,pageSize);
            IEnumerable<BlogPostDTO> dtos = posts.Select(p => p.ToDTO());
            return dtos;
        }

        public async Task<PagedList<BlogPostDTO>> GetPublishedPostsAsync(int page, int pageSize)
        {
            PagedList<BlogPost> posts = await _repository.GetPublishedPostsAsync(page, pageSize);
            PagedList<BlogPostDTO> postDTOs = new()
            {
				Page = posts.Page,
				TotalPages = posts.TotalPages,
				TotalItems = posts.TotalItems,
				Data = posts.Data.Select(p => p.ToDTO())

			};

            return postDTOs;
        }

        public async Task<TagDTO?> GetTagByIdAsync(int tagId)
        {
            Tag? tag = await _repository.GetTagByIdAsync(tagId);
            return tag?.ToDTO();
        }

        public async Task<IEnumerable<BlogPostDTO>> GetTopBlogPostsAsync(int count)
        {
            IEnumerable<BlogPost> blogPosts = await _repository.GetTopBlogPostsAsync(count);
            return blogPosts.Select(b => b.ToDTO());
        }

        public async Task PublishBlogPostAsync(int blogPostId)
        {
            await _repository.PublishBlogPostAsync(blogPostId);
        }

        public async Task RestoreBlogPostAsync(int blogPostId)
        {
            await _repository.RestoreBlogPostAsync(blogPostId);
        }

        public async Task<PagedList<BlogPostDTO>> SearchBlogPostsAsync(string query, int page, int pageSize)
        {
           PagedList<BlogPost> posts = await _repository.SearchBlogPostsAsync(query,page, pageSize);

            PagedList<BlogPostDTO> postDTOs = new()
            {
                Page = posts.Page,
                TotalPages = posts.TotalPages,
                TotalItems = posts.TotalItems,
                Data = posts.Data.Select(p => p.ToDTO())
            };
            
            return postDTOs;
        }

        public async Task UnpublishBlogPostAsync(int blogPostId)
        {
            await _repository.UnpublishBlogPostAsync(blogPostId);
        }

        public async Task UpdateBlogPostAsync(BlogPostDTO blogPost)
        {
            //Handle Tags
            await _repository.RemoveTagsFromBlogPostAsync(blogPost.Id);


            BlogPost? exisitingBlogPost = await _repository.GetBlogPostByIdAsync(blogPost.Id);

            if (exisitingBlogPost is null)
            {
                return;
            }

            exisitingBlogPost.Title = blogPost.Title;
            exisitingBlogPost.Slug = blogPost.Slug;
            exisitingBlogPost.Abstract = blogPost.Abstract;
            exisitingBlogPost.Content = blogPost.Content;
            exisitingBlogPost.IsPublished = blogPost.IsPublished;
            exisitingBlogPost.IsDeleted = blogPost.IsDeleted;
            exisitingBlogPost.CategoryId = blogPost.CategoryId;
            exisitingBlogPost.Category = null;
            exisitingBlogPost.Updated = DateTimeOffset.UtcNow;

            if (blogPost.ImageUrl?.StartsWith("data:") == true)
            {
                try
                {
                    exisitingBlogPost.Image = UploadHelper.GetImageUpload(blogPost.ImageUrl);
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex);
                }
            }

            else exisitingBlogPost.Image = null;


            await _repository.UpdateBlogPostAsync(exisitingBlogPost);
            
            //Replace Tags
            IEnumerable<string> tagNames = blogPost.Tags.Select(t => t.Name!);
            //Handle Tags Here
            await _repository.AddTagsToBlogPostAsync(exisitingBlogPost.Id, tagNames);
        }
    }
}
    
