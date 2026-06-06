using Microsoft.AspNetCore.Mvc;
using Social_Network.API.Services;

namespace Social_Network.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController: ControllerBase
    {
        private ICommentsService _comment;
        public CommentController(ICommentsService comment) => _comment = comment;

        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetComments([FromRoute] int postId)
        {
            var comments = await _comment.GetCommentsAsync(postId);
            return Ok(comments);
        }

        public record CreateCommentRequest(int UserId, int PostId, string Content);

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest req)
        {
            var com = await _comment.CreateCommentAsync(req.UserId, req.PostId, req.Content);
            return Ok(com);
        }

        [HttpDelete("{userId}/{commentId}")]
        public async Task<IActionResult> DeleteComment([FromRoute]int userId, [FromRoute] int commentId)
        {
            bool delcom = await _comment.DeleteCommentAsync(userId, commentId);
            if (!delcom) return NotFound();
            return NoContent();
        }
    }
}
