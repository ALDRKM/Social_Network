using Social_Network.Core.Models;

namespace Social_Network.Service
{
    public interface IPostService
    {
        Task<Post?> GetByIdAsync(int postId);
        Task<List<Post>> GetFeedAsync(int userId);
        Task<List<Post>> GetUserPostsAsync(int userId, PostType? type = null);
        Task<Post?> CreatePostAsync(int userId, string content, PostType type,
            List<string>? imageUrls = null, List<string>? tags = null, List<int>? mentionUserIds = null);
        Task DeletePostAsync(int userId, int postId);
        Task<List<Post>> SearchPostsAsync(string query);
        Task<List<Post>> SearchByTagAsync(string tag);
        Task<List<string>> SearchTagNamesAsync(string query);
    }
}
