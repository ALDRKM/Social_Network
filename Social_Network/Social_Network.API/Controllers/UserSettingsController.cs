using Microsoft.AspNetCore.Mvc;
using Social_Network.API.Services;

namespace Social_Network.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserSettingsController: ControllerBase
    {
        private readonly IUserSettingsService _userset;
        public UserSettingsController(IUserSettingsService userset) => _userset = userset;

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetSettings([FromRoute] int userId)
        {
            var settings = await _userset.GetByUserIdAsync(userId);
            if(settings == null) return NotFound();
            return Ok(settings);
        }

        public record UpdateSettingsRequest(bool IsPrivate, bool NotificationEnabled);

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateSettings([FromRoute]int userId, [FromBody] UpdateSettingsRequest req)
        {
            var settings = await _userset.UpdateAsync(userId, req.IsPrivate, req.NotificationEnabled);
            if(settings == null) return NotFound();
            return Ok(settings);
        }
    }
}
