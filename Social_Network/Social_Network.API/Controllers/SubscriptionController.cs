using Microsoft.AspNetCore.Mvc;
using Social_Network.API.Services;

namespace Social_Network.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _sub;

        public SubscriptionController(ISubscriptionService sub) => _sub = sub;

        public record FollowRequest(int FollowerId, int FollowingId);

        [HttpPost("follow")]
        public async Task<IActionResult> Follow([FromBody] FollowRequest follow) =>
            Ok(await _sub.FollowAsync(follow.FollowerId, follow.FollowingId));

        [HttpDelete("unfollow/{followerId}/{followingId}")]
        public async Task<IActionResult> UnFollow([FromRoute] int followerId, [FromRoute] int followingId)
        {
            await _sub.UnFollowAsync(followerId, followingId);
            return NoContent();
        }

        [HttpGet("isfollowing/{followerId}/{followingId}")]
        public async Task<IActionResult> IsFollowing([FromRoute] int followerId, [FromRoute] int followingId) =>
            Ok(await _sub.IsFollowingAsync(followerId, followingId));

        [HttpGet("pending/{followerId}/{followingId}")]
        public async Task<IActionResult> HasPending([FromRoute] int followerId, [FromRoute] int followingId) =>
            Ok(await _sub.HasPendingRequestAsync(followerId, followingId));

        [HttpGet("pending/following/{followerId}")]
        public async Task<IActionResult> PendingFollowingIds([FromRoute] int followerId) =>
            Ok(await _sub.GetPendingFollowingIdsAsync(followerId));

        [HttpPost("request/{requestId}/approve")]
        public async Task<IActionResult> Approve([FromRoute] int requestId, [FromQuery] int ownerId)
        {
            await _sub.ApproveRequestAsync(requestId, ownerId);
            return Ok();
        }

        [HttpPost("request/{requestId}/reject")]
        public async Task<IActionResult> Reject([FromRoute] int requestId, [FromQuery] int ownerId)
        {
            await _sub.RejectRequestAsync(requestId, ownerId);
            return Ok();
        }

        [HttpGet("followers/count/{userId}")]
        public async Task<IActionResult> FollowersCount([FromRoute] int userId) =>
            Ok(await _sub.GetFollowersCountAsync(userId));

        [HttpGet("following/count/{userId}")]
        public async Task<IActionResult> FollowingCount([FromRoute] int userId) =>
            Ok(await _sub.GetFollowingCountAsync(userId));

        [HttpGet("following/{userId}")]
        public async Task<IActionResult> FollowingIds([FromRoute] int userId) =>
            Ok(await _sub.GetFollowingIdsAsync(userId));
    }
}
