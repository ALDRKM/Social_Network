using Social_Network.Core.Models;


namespace Social_Network.API.Services
{
    public interface IPostService
    {
        Task<Post?> GetByIdAsync(int postId);
        Task<List<Post>> GetFeedAsync(int userId);
        Task<List<Post>> GetUserPostsAsync(int userId, PostType? type = null);
        Task<Post> CreatePostAsync(int userId, string content, PostType type,
            string? imageUrl = null, List<string>? tags = null, List<int>? mentionUserIds = null);
        Task DeletePostAsync(int userId, int postId);
        Task<List<Post>> SearchPostsAsync(string query);
        Task<List<Post>> SearchByTagAsync(string tag);
        Task<List<Post>> GetRecentAsync(int take = 50);
        Task<List<string>> SearchTagNamesAsync(string query, int take = 10);
    }
}
