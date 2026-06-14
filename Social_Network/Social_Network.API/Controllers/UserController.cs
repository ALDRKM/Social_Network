using Microsoft.AspNetCore.Mvc;
using Social_Network.API.Services;


namespace Social_Network.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _user;

        public UserController(IUserService user) => _user = user;

        public record GetUserProfileResponse(int Id, string Login, string? Bio, string? AvatarUrl, DateTime CreatedAt);

        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserProfileResponse>> GetUserProfile([FromRoute]int id)
        {
            var user = await _user.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            return Ok(new GetUserProfileResponse(user.Id, user.Login, user.Bio, user.AvatarUrl, user.CreatedAt));
        }

        public record UpdateUserProfileRequest( string Login, string? Bio, string? AvatarUrl);
        public record UpdateUserProfileResponse(int Id, string Login, string? Bio, string? AvatarUrl);

        [HttpPut("{id}")]
        public async Task<ActionResult<UpdateUserProfileResponse>> UpdateUserProfile([FromRoute] int id, [FromBody] UpdateUserProfileRequest req)
        {
            var user = await _user.UpdateUserProfileAsync(id, req.Login, req.Bio, req.AvatarUrl);
            if (user == null) return NotFound();

            return Ok( new UpdateUserProfileResponse(user.Id, user.Login, user.Bio, user.AvatarUrl));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount([FromRoute] int id)
        {
            bool del = await _user.DeleteAccountAsync(id);
            if (!del) return NotFound();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return BadRequest();
            var users = await _user.SearchUsersAsync(query);
            return Ok(users);
        }

    }
}
