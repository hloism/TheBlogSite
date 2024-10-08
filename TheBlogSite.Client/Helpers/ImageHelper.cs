﻿using Microsoft.AspNetCore.Components.Forms;

namespace TheBlogSite.Client.Helpers
{
    public static class ImageHelper
    {
        public static readonly string DefaultProfilePicture = "/Images/ProfileImage.jpg";
        public static readonly string DefaultBlogImage = "/images/DefaultBlogImage.png";
        public static readonly string DefaultCategoryImage = "/images/DefaultCategoryImage.jpg";
        public static int MaxFileSize = 5 * 1024 * 1024;

        public static async Task<string> GetDataUrl(IBrowserFile file)
        {
            using Stream fileStream = file.OpenReadStream(MaxFileSize);
            using MemoryStream ms = new MemoryStream();
            await fileStream.CopyToAsync(ms);

            byte[] imageBytes = ms.ToArray();
            string imageBase64 = Convert.ToBase64String(imageBytes);
            string dataUrl = $"data:{file.ContentType};base64,{imageBase64}";

            return dataUrl;
        }
    }
}

