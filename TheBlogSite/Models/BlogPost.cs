using System.ComponentModel.DataAnnotations;
using TheBlogSite.Data;
using TheBlogSite.Client.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Hosting;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Migrations.Operations;


namespace TheBlogSite.Models
{
    public class BlogPost
    {
        private DateTimeOffset _created;
        private DateTimeOffset? _updated;



        //Primary Key will NEVER be duplicated (remove Myproperty and enter Id)
        public int Id { get; set; }

        //Title of Blog & required, 2-200 characters
        [Required]
        [MinLength(2, ErrorMessage = "Blog Posts must have a title.")]
        [MaxLength(200, ErrorMessage = "The {0} must be less than {1} characters long.")]
        public string? Title { get; set; }

        //Slug(Required - human-readable URL)
        [Required]
        public string? Slug { get; set; }

        //Abstract (required (2-600 characters, Introductory paragraph to the post)
        [Required]
        [StringLength(600, MinimumLength = 2, ErrorMessage = "The {0} must be between {2} and {1} characters long.")]
        public string? Abstract { get; set; }

        [Required]
        public string? Content { get; set; }

        //When the post was authored Must be stored as UTC
        [Required]
        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }

        public DateTimeOffset? Updated
        {
            get => _updated;
            set => _updated = value?.ToUniversalTime();
        }


        public bool IsDeleted { get; set; }

        public bool IsPublished { get; set; }

        // One to One
        //Relationships/Navigations Properties
        public Guid? ImageId { get; set; }
        public virtual ImageUpload? Image { get; set; }

        [NotMapped]
        public string? ImageURL => ImageId.HasValue ? $"api/uploads/{ImageId}" : null;


        //Many to One

        //Category - blog posts must belong to one category
        public int CategoryId { get; set; }

        public virtual Category? Category { get; set; }


        //One To Many
        //Comments - blog posts may have many comments
        public virtual ICollection<Comment> Comments { get; set; } = [];


        //Many to Many    
        //Tags - blog posts may have many tags
        public virtual ICollection<Tag> Tags { get; set; } = [];
    }

    public static class BlogPostExtensions
    {
        public static BlogPostDTO ToDTO(this BlogPost post)
        {
            ICollection<TagDTO> tags = [..post.Tags.Select(t =>
            {
                t.Posts = [];
                return t.ToDTO();
            }
            )];

            CategoryDTO? category = null;

            if (post.Category != null)
            {
                post.Category.Posts = [];
                category = post.Category.ToDTO();
            }


            return new BlogPostDTO()
            {
                Id = post.Id,
                Slug = post.Slug,
                //BlogPost Properties
                Title = post.Title,
                Abstract = post.Abstract,
                Category = category,
                Comments = [..post.Comments.Select(c => c.ToDTO())],
                Tags = tags,
                IsPublished = post.IsPublished,
                IsDeleted = post.IsDeleted,
                Content = post.Content,
                ImageUrl = post.ImageURL,
                CategoryId = post.CategoryId,
                Created = post.Created,
            };
        }
    }
}
