using System.ComponentModel.DataAnnotations;

namespace TheBlogSite.Models
{
	public class ImageUpload
	{	
		//properties
		public Guid Id { get; set; }

		[Required]
		public byte[]? Data { get; set; }

		[Required]
		public string? Extension { get; set; }


	}
}
