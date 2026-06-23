using Microsoft.AspNetCore.Mvc;
using Social_Network.API.Services;

namespace Social_Network.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SavedPostController : ControllerBase
    {
        private readonly ISavedPostService _saved;
        public SavedPostController(ISavedPostService saved) => _saved = saved;

        [HttpPost("{userId}/{postId}")]
        public async Task<IActionResult> Save([FromRoute] int userId, [FromRoute] int postId)
        {
            await _saved.SaveAsync(userId, postId);
            return Ok();
        }

        [HttpDelete("{userId}/{postId}")]
        public async Task<IActionResult> Unsave([FromRoute] int userId, [FromRoute] int postId)
        {
            await _saved.UnsaveAsync(userId, postId);
            return NoContent();
        }

        [HttpGet("issaved/{userId}/{postId}")]
        public async Task<IActionResult> IsSaved([FromRoute] int userId, [FromRoute] int postId)
        {
            bool saved = await _saved.IsSavedAsync(userId, postId);
            return Ok(saved);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetSaved([FromRoute] int userId)
        {
            var posts = await _saved.GetSavedAsync(userId);
            return Ok(posts);
        }

        [HttpGet("ids/{userId}")]
        public async Task<IActionResult> GetSavedIds([FromRoute] int userId)
        {
            var ids = await _saved.GetSavedIdsAsync(userId);
            return Ok(ids);
        }
    }
}
