using Microsoft.EntityFrameworkCore;
using TheBlogSite.Data;
using TheBlogSite.Services.Interfaces;
using TheBlogSite.Models;

namespace TheBlogSite.Services.Repository
{
    public class CommentRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : ICommentRepository
    {
        //done
        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            context.Comments.Add(comment);
            await context.SaveChangesAsync();
            return comment;

        }
        //done
        public async Task DeleteCommentAsync(int commentId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            Comment? comment = await context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment is not null)
            {
                context.Comments.Remove(comment);
                await context.SaveChangesAsync();
            }
        }
        //done
        public async Task<Comment?> GetCommentByIdAsync(int commentId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            Comment? comment = await context.Comments
                                            .Include(c => c.Author)
                                            .FirstOrDefaultAsync(c => c.Id == commentId);
            return comment;
        }
        //done
        public async Task<IEnumerable<Comment>> GetCommentsAsync(int postId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            List<Comment> comments = await context.Comments
            .Where(c => c.BlogPostId == postId)
            .Include(c => c.Author)
            .ToListAsync();

            return comments;
        }
        //done
        public async Task<IEnumerable<Category>> GetCategoriesAsync(int count)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            List<Category> categories = await context.Categories
                .Include(c => c.Posts.Where(p => p.IsPublished == true && p.IsDeleted == false))
                .OrderByDescending(c => c.Posts.Where(p => p.IsPublished == true && p.IsDeleted == false)
                .Count())
                .Take(count)
                .ToListAsync();

            return categories;

        }

        public async Task UpdateCommentAsync(Comment comment)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            if (await context.Comments.AnyAsync(c => c.Id == comment.Id))
            {
                context.Comments.Update(comment);
                await context.SaveChangesAsync();
            }
        }
    }
}
