

namespace Social_Network.Service
{
    public interface ISubscriptionService
    {
        Task FollowAsync(int followerId, int followingId);
        Task UnfollowAsync(int followerId, int followingId);
        Task<bool> IsFollowingAsync(int followerId, int followingId);
        Task<int> GetFollowersCountAsync(int userId);
        Task<int> GetFollowingCountAsync(int userId);
        Task<List<int>> GetFollowingIdsAsync(int userId);
    }
}
