using System.ComponentModel.DataAnnotations;
using TheBlogSite.Client.Models;

namespace TheBlogSite.Models
{
    public class Tag
    {

        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tag names must be between {2} and {1} characters long")]

        public string? Name { get; set; }

        //navigation properties 

        public virtual ICollection<BlogPost> Posts { get; set; } = [];

    }

    public static class TagExtensions
    {
        public static TagDTO ToDTO(this Tag tag)
        {
            return new TagDTO()
            {
                Id = tag.Id,
                Name = tag.Name,
                Posts = [.. tag.Posts.Select(p => p.ToDTO())]
            };
        }
    }
}
