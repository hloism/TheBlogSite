using TheBlogSite.Client.Models;
using TheBlogSite.Client.Services.Interfaces;
using System.Net.Http.Json;

namespace TheBlogSite.Client.Services
{
    //delete page
    public class BlogPostService : IBlogPostService
    {
        private readonly HttpClient _httpClient;

        public BlogPostService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<BlogPostDTO> CreateBlogPostAsync(BlogPostDTO blogPost)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/blogPosts", blogPost);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<BlogPostDTO>()
               ?? throw new HttpRequestException("Invalid JSON recieved from server");
        }

        public async Task DeleteBlogPostAsync(int blogPostId)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"api/blogPosts/{blogPostId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<BlogPostDTO?> GetBlogPostByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<BlogPostDTO>($"api/blogPosts/{id}");
        }

        public async Task<BlogPostDTO?> GetBlogPostBySlugAsync(string slug)
        {
            return await _httpClient.GetFromJsonAsync<BlogPostDTO>($"api/blogPosts?slug=/{slug}");
        }

        public async Task<IEnumerable<BlogPostDTO>> GetDeletedPostsAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<BlogPostDTO>>("api/BlogPost") ?? [];
        }

        public async Task<IEnumerable<BlogPostDTO>> GetDraftPostsAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<BlogPostDTO>>("api/BlogPost") ?? [];
        }

        public Task<IEnumerable<BlogPostDTO>> GetPostsByCategoryId(int categoryId)
        {
            throw new NotImplementedException();
        }

		public Task<IEnumerable<BlogPostDTO>> GetPostsByTagIdAsync(int tagId, int page, int pageSize)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<BlogPostDTO>> GetPublishedPostsAsync()
        {
            throw new NotImplementedException();
        }

		public Task<TagDTO?> GetTagByIdAsync(int tagId)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<BlogPostDTO>> GetTopBlogPostsAsync(int count)
        {
            throw new NotImplementedException();
        }

        public Task PublishBlogPostAsync(int blogPostId)
        {
            throw new NotImplementedException();
        }


        public Task RestoreBlogPostAsync(int blogPostId)
        {
            throw new NotImplementedException();
        }

        public Task UnpublishBlogPostAsync(int blogPostId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateBlogPostAsync(BlogPostDTO blogPost)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/BlogPost/{blogPost.Id}", blogPost);
            response.EnsureSuccessStatusCode();
        }
    }
}