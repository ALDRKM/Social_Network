namespace Social_Network.API.Services
{
    public interface ILikeService
    {
        Task CreateLikeAsync(int userId, int postId);
        Task DeleteLikeAsync(int userId, int postId);
        Task<int> GetCountLikeAsync(int postId);
        Task<bool> IsLikeAsync(int userId, int postId);
        Task<List<Core.Models.Post>> GetUserLikedPostsAsync(int userId);
    }
}
