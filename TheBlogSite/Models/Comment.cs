using System.ComponentModel.DataAnnotations;
using TheBlogSite.Data;
using TheBlogSite.Client.Models;
using Microsoft.CodeAnalysis.CSharp;
using TheBlogSite.Client.Helpers;


namespace TheBlogSite.Models
{
    public class Comment
    {
        private DateTimeOffset _created;
        private DateTimeOffset? _updated;

        public int Id { get; set; }

        [Required]
        [StringLength(5000, MinimumLength = 2, ErrorMessage = "Comments must be between {2} and {1} characters long")]

        public string? Content { get; set; }

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

        [MaxLength(200)]

        public string? UpdateReason { get; set; }

        //navigation properties
        [Required]
        public string? AuthorId { get; set; }

        public virtual ApplicationUser? Author { get; set; }

        public int BlogPostId { get; set; }

        public virtual BlogPost? BlogPost { get; set; }
    }

    public static class CommentExtensions
    {
        public static CommentDTO ToDTO(this Comment comment)
        {
            return new CommentDTO()
            {
                Id = comment.Id,
                Content = comment.Content,
                Created = comment.Created,
                Updated = comment.Updated,
                UpdateReason = comment.UpdateReason,
                AuthorId = comment.AuthorId,
                AuthorName = comment.Author?.FullName,
                AuthorImageUrl = comment.Author?.ImageId is not null ? $"api/uploads/{comment.Author.ImageId}" : ImageHelper.DefaultProfilePicture,
                BlogPostId = comment.BlogPostId,
            };
        }
    }
}
