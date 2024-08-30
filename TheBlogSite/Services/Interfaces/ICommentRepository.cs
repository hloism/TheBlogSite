using TheBlogSite.Client.Models;
using TheBlogSite.Models;

namespace TheBlogSite.Services.Interfaces
{
    public interface ICommentRepository
    {
        Task<Comment> CreateCommentAsync(Comment comment);
        Task UpdateCommentAsync(Comment comment);
        Task DeleteCommentAsync(int commentId);

        Task<Comment?> GetCommentByIdAsync(int commentId);
        Task<IEnumerable<Comment>> GetCommentsAsync(int postId);

    }
}
