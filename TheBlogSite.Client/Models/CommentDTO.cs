using System.ComponentModel.DataAnnotations;

namespace TheBlogSite.Client.Models
{
    public class CommentDTO
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

        //Navigation Properties
        public string? AuthorName { get; set; }

        public string? AuthorImageUrl { get; set; }

        public string? AuthorId { get; set; }


        public int BlogPostId { get; set; }
    }
}
