namespace Social_Network.API.Services
{
    public interface ISubscriptionService
    {
        Task FollowAsync(int followerId, int followingId);
        Task UnFollowAsync(int followerId, int followingId);
        Task<bool> IsFollowingAsync(int followerId, int followingId);
        Task<int> GetFollowersCountAsync(int userId);
        Task<int> GetFollowingCountAsync(int userId);
        Task<List<int>> GetFollowingIdsAsync(int userId);
    }
}
