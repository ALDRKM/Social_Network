using Social_Network.Core.Models;

namespace Social_Network.Service
{
    public interface ISavedPostService
    {
        Task SaveAsync(int userId, int postId);
        Task UnsaveAsync(int userId, int postId);
        Task<bool> IsSavedAsync(int userId, int postId);
        Task<List<Post>> GetSavedAsync(int userId);
        Task<List<int>> GetSavedIdsAsync(int userId);
    }
}
