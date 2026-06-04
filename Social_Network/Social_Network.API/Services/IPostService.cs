using Social_Network.Core.Models;


namespace Social_Network.API.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetFeedAsync(int userId);
        Task<List<Post>> GetUserPostsAsync(int userId);
        Task<Post> CreatePostAsync(int userId,string content, string? imageUrl);
        Task DeletePostAsync(int userId, int postId);
    }
}
