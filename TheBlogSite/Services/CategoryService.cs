using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using TheBlogSite.Client.Models;
using TheBlogSite.Client.Services.Interfaces;
using TheBlogSite.Helpers;
using TheBlogSite.Models;
using TheBlogSite.Services.Interfaces;
using System.Data;
using System.Net.Http;
using TheBlogSite.Services.Repository;

namespace TheBlogSite.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<CategoryDTO> CreateCategoryAsync(CategoryDTO category)
        {
            Category newCategory = new Category()
            {
                Name = category.Name,
                Description = category.Description,
            };

            if (category.ImageUrl.StartsWith("data:"))
            {
                try
                {
                    newCategory.Image = UploadHelper.GetImageUpload(category.ImageUrl);
                }
                catch
                {
                    newCategory.Image = null;
                }
            }

            newCategory = await _repository.CreateCategoryAsync(newCategory);
            return newCategory.ToDTO();
        }


        public async Task<IEnumerable<CategoryDTO>> GetCategoriesAsync()
        {
            IEnumerable<Category> categories = await _repository.GetCategoriesAsync();

            return categories.Select(c => c.ToDTO());
        }


        public async Task<CategoryDTO?> GetCategoryByIdAsync(int id)
        {
            Category? category = await _repository.GetCategoryByIdAsync(id);
            return category?.ToDTO();
        }

        public async Task UpdateCategoryAsync(CategoryDTO category)
        {
            var existingCategory = await _repository.GetCategoryByIdAsync(category.Id);
            if (existingCategory is null)
            {
                return; 
            
            }
               
            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;

            if (category.ImageUrl.StartsWith("data:"))
            {
                existingCategory.Image = UploadHelper.GetImageUpload(category.ImageUrl);
            }
            else
            {
                existingCategory.Image = null;
            }

            await _repository.UpdateCategoryAsync(existingCategory);
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            await _repository.DeleteCategoryAsync(categoryId);
        }

        public async Task<IEnumerable<CategoryDTO>> GetTopCategoriesAsync(int count)
        {
                IEnumerable<Category> categories = await _repository.GetTopCategoriesAsync(count);
                return categories.Select(c => c.ToDTO());
        }
        
    }
}
