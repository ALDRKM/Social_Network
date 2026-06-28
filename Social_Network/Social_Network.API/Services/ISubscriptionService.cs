using Social_Network.Core.Models.DTOs;

namespace Social_Network.API.Services
{
    public interface ISubscriptionService
    {
        Task<FollowResultDto> FollowAsync(int followerId, int followingId);
        Task UnFollowAsync(int followerId, int followingId);
        Task<bool> IsFollowingAsync(int followerId, int followingId);
        Task<bool> HasPendingRequestAsync(int followerId, int followingId);
        Task ApproveRequestAsync(int requestId, int ownerId);
        Task RejectRequestAsync(int requestId, int ownerId);
        Task<List<int>> GetPendingFollowingIdsAsync(int followerId);
        Task<int> GetFollowersCountAsync(int userId);
        Task<int> GetFollowingCountAsync(int userId);
        Task<List<int>> GetFollowingIdsAsync(int userId);
    }
}
