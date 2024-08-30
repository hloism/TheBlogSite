
using TheBlogSite.Client.Models;
using TheBlogSite.Client.Services.Interfaces;
using System.Net.Http.Json;

namespace TheBlogSite.Client.Services
{
    public class CommentService : ICommentService
    {
        private readonly HttpClient _httpClient;

        public CommentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CommentDTO> CreateCommentAsync(CommentDTO comment)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/comments", comment);
            response.EnsureSuccessStatusCode();

            CommentDTO? createdComment = await response.Content.ReadFromJsonAsync<CommentDTO>();
            return createdComment!;
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"api/comments/{commentId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<CommentDTO?> GetCommentByIdAsync(int commentId)
        {
			CommentDTO? comment = await _httpClient.GetFromJsonAsync<CommentDTO>($"api/comments/{commentId}");

            return comment!;
        }

        public async Task<IEnumerable<CommentDTO>> GetCommentsAsync(int postId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CommentDTO>>("api/comments") ?? [];
        }

        public async Task UpdateCommentAsync(CommentDTO commentDTO)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/contactService/{commentDTO.Id}", commentDTO);
            response.EnsureSuccessStatusCode();

        }
    }
}
