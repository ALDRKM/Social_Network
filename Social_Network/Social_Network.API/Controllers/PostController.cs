using Microsoft.AspNetCore.Mvc;
using Social_Network.API.Services;


namespace Social_Network.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _post;
        public PostController(IPostService post) => _post = post;

        [HttpGet("feed/{userId}")]
        public async Task<IActionResult> GetFeed([FromRoute]int userId)
        {
            var ListPost = await _post.GetFeedAsync(userId);
            return Ok(ListPost);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserPosts([FromRoute] int userId)
        {
            var ListPost = await _post.GetUserPostsAsync(userId);
            return Ok(ListPost);
        }

        public record CreatePostRequest(int UserId, string Content, string? ImageUrl);

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest p)
        {
            var post = await _post.CreatePostAsync(p.UserId, p.Content, p.ImageUrl);
            return Ok(post);
        }

        [HttpDelete("{userId}/{postId}")]
        public async Task<IActionResult> DeletePost([FromRoute] int userId, [FromRoute] int postId)
        {
            await _post.DeletePostAsync(userId, postId);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchPosts([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return BadRequest();

            var posts = await _post.SearchPostsAsync(query);
            return Ok(posts);
        }
    }
}
