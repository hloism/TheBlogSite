using TheBlogSite.Client.Models;

namespace TheBlogSite.Client.Services.Interfaces
{
    public interface IBlogPostService
    {
        Task<BlogPostDTO>CreateBlogPostAsync(BlogPostDTO blogPostDTO);
        Task UpdateBlogPostAsync(BlogPostDTO blogPost);
        Task<PagedList<BlogPostDTO>> GetPublishedPostsAsync(int page, int pageSize);
        Task<PagedList<BlogPostDTO>> GetPostsByCategoryId(int categoryId, int page, int pageSize);
        Task<IEnumerable<BlogPostDTO>> GetDraftPostsAsync();
        Task<IEnumerable<BlogPostDTO>> GetDeletedPostsAsync();
        Task<BlogPostDTO?> GetBlogPostBySlugAsync(string slug);
        Task<BlogPostDTO?> GetBlogPostByIdAsync(int id);
        Task<IEnumerable<BlogPostDTO>> GetTopBlogPostsAsync(int count);
        Task<PagedList<BlogPostDTO>> SearchBlogPostsAsync(string query, int page, int pageSize);
        Task DeleteBlogPostAsync(int blogPostId);
        Task RestoreBlogPostAsync(int blogPostId);
        Task PublishBlogPostAsync(int blogPostId);
        Task UnpublishBlogPostAsync(int blogPostId);
        Task<TagDTO?> GetTagByIdAsync(int tagId);
        Task<IEnumerable<BlogPostDTO>> GetPostsByTagIdAsync(int tagId, int page, int pageSize);

    }


}
