using System.ComponentModel.DataAnnotations;
using TheBlogSite.Client.Helpers;

namespace TheBlogSite.Client.Models
{
    public class CategoryDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The {} must be between {} and {} characters long")]

        public string? Name { get; set; }

        [MaxLength(200)]

        public string? Description { get; set; }

        //Navigation Properties

        public string ImageUrl { get; set; } = ImageHelper.DefaultCategoryImage;

        public ICollection<BlogPostDTO> Posts { get; set; } = [];
    }
}
