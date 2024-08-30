using TheBlogSite.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheBlogSite.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [NotMapped]
        //two different ways to show FullName 
        //public string? FullName { get { return $"{FirstName} {LastName}"; } }

        public string? FullName => $"{FirstName} {LastName}";

        //virtual -- obtain data between models
        public Guid? ImageId { get; set; }

        public virtual ImageUpload? Image { get; set; }

        //Add relationship to Comments model 

        public virtual ICollection<Comment> Comments { get; set; } = [];
    }

}
