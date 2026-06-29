using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Social_Network.API.Services;
using Social_Network.Core.Models;


namespace Social_Network.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _post;
        public PostController(IPostService post) => _post = post;



        [HttpGet("{postId}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute]int postId)
        {
            var post = await _post.GetByIdAsync(postId);
            if (post == null) return NotFound();
            return Ok(post);
        }

        [HttpGet("feed/{userId}")]
        public async Task<IActionResult> GetFeed([FromRoute] int userId, [FromQuery] string mode = "all")
        {
            var listPost = await _post.GetFeedAsync(userId, mode);
            return Ok(listPost);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserPosts([FromRoute] int userId, [FromQuery] PostType? type)
        {
            var ListPost = await _post.GetUserPostsAsync(userId, type);
            return Ok(ListPost);
        }

        public record CreatePostRequest(
            int UserId,
            string Content,
            PostType Type,
            string? ImageUrl,
            List<string>? Tags,
            List<int>? MentionUserIds);

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest p)
        {
            var post = await _post.CreatePostAsync(p.UserId, p.Content, p.Type, p.ImageUrl, p.Tags, p.MentionUserIds);
            return Ok(post);
        }

        [HttpGet("tag/{tag}")]
        public async Task<IActionResult> SearchByTag([FromRoute] string tag)
        {
            var posts = await _post.SearchByTagAsync(tag);
            return Ok(posts);
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecent()
        {
            var posts = await _post.GetRecentAsync();
            return Ok(posts);
        }

        [HttpGet("tags/search")]
        public async Task<IActionResult> SearchTags([FromQuery] string query)
            => Ok(await _post.SearchTagNamesAsync(query ?? string.Empty));

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
