using System.ComponentModel.DataAnnotations;
using TheBlogSite.Components.Pages;
using TheBlogSite.Data;
using TheBlogSite.Client.Models;
using TheBlogSite.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheBlogSite.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The {0} must be between {2} and {1} characters long")]

        public string? Name { get; set; }

        [MaxLength(200)]

        public string? Description { get; set; }

        //navigation properties

        public Guid? ImageId { get; set; }
        public virtual ImageUpload? Image { get; set; }

        [NotMapped]
        public string ImageUrl => ImageId.HasValue ? $"api/uploads/{ImageId}" : UploadHelper.DefaultCategoryImage;

        public virtual ICollection<BlogPost> Posts { get; set; } = [];
    }

    public static class CategoryItemExtension
    {
        public static CategoryDTO ToDTO(this Category category)
        {
            return new CategoryDTO()
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                Posts = [.. category.Posts.Select(p => p.ToDTO())]

            };
        }

    }
}
