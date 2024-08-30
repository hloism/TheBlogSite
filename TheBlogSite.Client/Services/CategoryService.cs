using TheBlogSite.Client.Models;
using TheBlogSite.Client.Services.Interfaces;
using System.Net.Http.Json;

namespace TheBlogSite.Client.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;

        public CategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CategoryDTO> CreateCategoryAsync(CategoryDTO category)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/categories", category);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CategoryDTO>()
               ?? throw new HttpRequestException("Invalid JSON recieved from server");
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"api/categories/{categoryId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<CategoryDTO>> GetCategoriesAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CategoryDTO>>("api/categories") ?? [];
        }

        public async Task<CategoryDTO?> GetCategoryByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<CategoryDTO>($"api/categories/{id}");
        }

        public Task<IEnumerable<CategoryDTO>> GetTopCategoriesAsync(int count)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateCategoryAsync(CategoryDTO category)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/categories/{category.Id}", category);
            response.EnsureSuccessStatusCode();
        }
    }
}
