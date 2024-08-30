using System.ComponentModel.DataAnnotations;

namespace TheBlogSite.Client.Models
{
    public class BlogPostDTO
    {
        private DateTimeOffset _created;
        private DateTimeOffset _updated;

        public int Id { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "Blog Posts must have a title")]
        [MaxLength(200, ErrorMessage = "The {0} must be less than {1} characters long")]

		public string? Title { get; set; }

        public string? Slug { get; set; }

        [Required]
        [StringLength(600, MinimumLength = 2, ErrorMessage = "The {0} must be at least {1} characters long")]

        public string? Abstract { get; set; }

        [Required]

        public string? Content { get; set; }

        public DateTimeOffset Created
        {
            get => _created.ToLocalTime();
            set => _created = value.ToUniversalTime();
        }

        public DateTimeOffset Updated
        {
            get => _updated.ToLocalTime();
            set => _updated = value.ToUniversalTime();
        }

        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }
        public string? ImageUrl { get; set; }

        public int CategoryId { get; set; }

        public virtual CategoryDTO? Category { get; set; }

        public virtual ICollection<CommentDTO> Comments { get; set; } = [];

        public virtual ICollection<TagDTO> Tags { get; set; } = [];

		public virtual ICollection<CategoryDTO> Categories { get; set; } = []; //copy this from CategoryDTO.cs change prefixes
	}
}
