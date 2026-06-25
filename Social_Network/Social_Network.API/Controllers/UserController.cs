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

        public record GetUserProfileResponse(int Id, string Login, string Email, string? Bio,
            string? AvatarUrl, DateTime CreatedAt, bool IsOnline, DateTime? LastSeen);

        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserProfileResponse>> GetUserProfile([FromRoute]int id)
        {
            var user = await _user.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            return Ok(new GetUserProfileResponse(user.Id, user.Login, user.Email, user.Bio,
                user.AvatarUrl, user.CreatedAt, user.IsOnline, user.LastSeen));
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

        public record UpdateAccountRequest(string Login, string Email);

        [HttpPut("{id}/account")]
        public async Task<IActionResult> UpdateAccount([FromRoute] int id, [FromBody] UpdateAccountRequest req)
        {
            var user = await _user.UpdateAccountAsync(id, req.Login, req.Email);
            if (user == null) return BadRequest("Не удалось обновить аккаунт (возможно, email занят).");
            return Ok(new { user.Id, user.Login, user.Email });
        }

        public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

        [HttpPut("{id}/password")]
        public async Task<IActionResult> ChangePassword([FromRoute] int id, [FromBody] ChangePasswordRequest req)
        {
            bool ok = await _user.ChangePasswordAsync(id, req.CurrentPassword, req.NewPassword);
            if (!ok) return BadRequest("Неверный текущий пароль.");
            return NoContent();
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
