using Social_Network.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Social_Network.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LikeController: ControllerBase
    {
        private readonly ILikeService _like;
        public LikeController(ILikeService like) => _like = like;

        [HttpPost("{userId}/{postId}")]
        public async Task<IActionResult> Like([FromRoute]int userId, [FromRoute] int postId)
        {
            await _like.CreateLikeAsync(userId, postId);
            return Ok();
        }

        [HttpDelete("{userId}/{postId}")]
        public async Task<IActionResult> UnLike([FromRoute]int userId, [FromRoute] int postId)
        {
            await _like.DeleteLikeAsync(userId, postId);
            return NoContent();
        }

        [HttpGet("count/{postId}")]
        public async Task<IActionResult> GetCount([FromRoute] int postId)
        {
            int count = await _like.GetCountLikeAsync(postId);
            return Ok(count);
        }

        [HttpGet("isliked/{userId}/{postId}")]
        public async Task<IActionResult> IsLike([FromRoute]int userId, [FromRoute]int postId)
        {
            bool islike = await _like.IsLikeAsync(userId, postId);
            return Ok(islike);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserLiked([FromRoute] int userId)
        {
            var posts = await _like.GetUserLikedPostsAsync(userId);
            return Ok(posts);
        }
    }
}
