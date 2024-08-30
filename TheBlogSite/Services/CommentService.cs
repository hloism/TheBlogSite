using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using TheBlogSite.Client.Models;
using TheBlogSite.Client.Services.Interfaces;
using TheBlogSite.Data;
using TheBlogSite.Services.Interfaces;
using TheBlogSite.Models;
using Microsoft.AspNetCore.Identity;

namespace TheBlogSite.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _repository;

        public CommentService(ICommentRepository repository)
        {
            _repository = repository;
        }

        public async Task<CommentDTO> CreateCommentAsync(CommentDTO comment)
        {
            Comment newComment = new Comment()
            {
                Content = comment.Content,
                AuthorId = comment.AuthorId,
                BlogPostId = comment.BlogPostId,
                Created = DateTime.UtcNow,
        
            };

            newComment = await _repository.CreateCommentAsync(newComment);

            return newComment.ToDTO();
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            await _repository.DeleteCommentAsync(commentId);
        }

        public async Task<CommentDTO?> GetCommentByIdAsync(int commentId)
        {
            Comment? comment = await _repository.GetCommentByIdAsync(commentId);
            return comment?.ToDTO();
        }

        public async Task<IEnumerable<CommentDTO>> GetCommentsAsync(int postId)
        {
            IEnumerable<Comment> comments = await _repository.GetCommentsAsync(postId);

            return comments. Select(c => c.ToDTO());
        }

        public async Task UpdateCommentAsync(CommentDTO commentDTO)
        {
            Comment? exisitngComment = await _repository.GetCommentByIdAsync(commentDTO.Id);


            if (exisitngComment is not null)
            {
                exisitngComment.Content = commentDTO.Content;
                exisitngComment.Updated = DateTime.UtcNow;
                exisitngComment.UpdateReason = commentDTO.UpdateReason;

                await _repository.UpdateCommentAsync(exisitngComment);
            }
        }
    }
}
