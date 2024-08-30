using System.ComponentModel.DataAnnotations;

namespace TheBlogSite.Client.Models
{
    public class TaskerItemDTO
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Every Task must have a name.")]
        public string? Name { get; set; }

        public bool IsComplete { get; set; }
    }
}
