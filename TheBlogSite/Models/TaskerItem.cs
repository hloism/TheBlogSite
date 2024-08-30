using TheBlogSite.Client.Models;
using TheBlogSite.Data;
using System.ComponentModel.DataAnnotations;

namespace TheBlogSite.Models
{
    public class TaskerItem
    {
        //every item needs to be identified with guid
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Every task must have a name.")]
        public string? Name { get; set; }

        public bool IsComplete { get; set; }

        //similar to fav movie
        [Required]
        public string? UserId { get; set; }

        //verify user exist befiore writing a new record on table, virtual EntityFrameowrk can modify it allow for 'lazy loading' vallid UserId get Task Item can say contact info and get it for me.  
        public virtual ApplicationUser? User { get; set; }
    }

    public static class TaskerItemExtension
    {   

        public static TaskerItemDTO ToDTO(this TaskerItem item) => new TaskerItemDTO
        {
            Id = item.Id,
            Name = item.Name,
            IsComplete = item.IsComplete

        };
    }
}
