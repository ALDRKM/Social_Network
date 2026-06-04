using Microsoft.AspNetCore.Mvc;
using Social_Network.API.Services;



namespace Social_Network.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController:ControllerBase
    {
        private readonly ISubscriptionService _sub;
        public SubscriptionController(ISubscriptionService sub) => _sub = sub;

        public record FollowRequest(int FollowerId, int FollowingId);

        [HttpPost("follow")]
        public async Task<IActionResult> Follow([FromBody] FollowRequest follow)
        {
            await _sub.FollowAsync(follow.FollowerId, follow.FollowingId);
            return Ok();
        }

        [HttpDelete("unfollow/{followerId}/{followingId}")]
        public async Task<IActionResult> UnFollow([FromRoute] int followerId, [FromRoute] int followingId )
        {
            await _sub.UnFollowAsync(followerId, followingId);
            return NoContent();
        }

        [HttpGet("isfollowing/{followerId}/{followingId}")]
        public async Task<IActionResult> IsFollowing([FromRoute] int followerId, [FromRoute] int followingId) => Ok(
            await _sub.IsFollowingAsync(followerId, followingId));
    }
}
