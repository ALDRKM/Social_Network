
namespace Social_Network.Service
{
    public interface ILikeService
    {
        Task LikeAsync(int userId, int postId);
        Task UnlikeAsync(int userId, int postId);
        Task<bool> IsLikeAsync(int userId, int postId);
        Task<int> GetLikeCountAsync(int postId);
    }
}
