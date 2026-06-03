using Social_Network.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Social_Network.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController: ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth) => _auth = auth;

        public record RegisterRequest(string Login, string Email, string Password);
        public record RegisterResponse(int Id, string Login, string Email);

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req )
        {
            var user = await _auth.RegisterAsync(req.Login, req.Email, req.Password);
            if (user == null) return BadRequest("Email уже занят");

            return Ok(new RegisterResponse( user.Id, user.Login, user.Email ));
        }

        public record LoginRequest(string Email, string Password);
        public record LoginResponse(int Id , string Login, string Email, string? AvatarUrl);

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var user = await _auth.LoginAsync(req.Email, req.Password);
            if (user == null) return Unauthorized("Неверный email или пароль");

            return Ok(new LoginResponse(user.Id, user.Login, user.Email, user.AvatarUrl));

        }

    }
}
