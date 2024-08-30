using System.ComponentModel.DataAnnotations;
using TheBlogSite.Client.Models;

namespace TheBlogSite.Client.Models
{
    public class TagDTO
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tag names must be between {2} and {1} charactes long")]
        public string? Name { get; set; }

        //navigation properties

        public ICollection<BlogPostDTO> Posts { get; set; } = [];

    }
}
