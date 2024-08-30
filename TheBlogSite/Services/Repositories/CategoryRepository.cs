using Microsoft.EntityFrameworkCore.Internal;
using TheBlogSite.Client.Models;
using TheBlogSite.Data;
using TheBlogSite.Models;
using TheBlogSite.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TheBlogSite.Services.Repository
{
    public class CategoryRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : ICategoryRepository
    {
    
        public async Task<Category> CreateCategoryAsync(Category category)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            context.Add(category);
            await context.SaveChangesAsync();

            return category;
        }


        public async Task DeleteCategoryAsync(int categoryId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            var category = await context.Categories
                .Include(c => c.Image)
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category != null)
            {
                context.Categories.Remove(category);
                if (category.Image is not null) context.Images.Remove(category.Image);
                await context.SaveChangesAsync();
            }
        }


        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            List<Category> categories = await context.Categories
                .Include(c => c.Posts)
                .ToListAsync();

            return categories;
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            return await context.Categories
            .Include(c => c.Posts)
            .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            ImageUpload? existingImage = null;

            if (category.Image is not null)
            {
                existingImage = await context.Images.FirstOrDefaultAsync(i => i.Id == category.ImageId);

                category.ImageId = category.Image.Id;
                context.Images.Add(category.Image);
            }

            context.Categories.Update(category);
            await context.SaveChangesAsync();

            if (existingImage is not null)
            {
                context.Remove(existingImage);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Category>> GetTopCategoriesAsync(int count)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            List<Category> categories = await context.Categories
                .Include(c => c.Posts.Where(p => p.IsPublished == true && p.IsDeleted == false))
                //.Include(c => c.Posts.Where(p => p.IsPublished && !p.IsDeleted))
                .OrderByDescending(c => c.Posts.Where(p => p.IsPublished == true && p.IsDeleted == false)
                //.OrderByDescending(c => c.Posts.Where(p => p.IsPublished && !p.IsDeleted))
                .Count())
                .Take(count)
                .ToListAsync();

            return categories;
        }
    }
}

