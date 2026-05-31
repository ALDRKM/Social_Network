namespace Social_Network.API.Services
{
    public interface ISubscriptionService
    {
        Task FollowAsync(int followerId, int followingId);
        Task UnFollowAsync(int followerId, int followingId);
        Task<bool> IsFollow(int followerId, int followingId);
    }
}
