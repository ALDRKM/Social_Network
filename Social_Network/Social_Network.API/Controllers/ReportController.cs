using Microsoft.AspNetCore.Mvc;
using Social_Network.API.Services;

namespace Social_Network.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _report;
        public ReportController(IReportService report) => _report = report;

        [HttpGet("top-posts")]
        public async Task<IActionResult> TopPosts([FromQuery] DateTime? from, [FromQuery] DateTime? to)
            => Ok(await _report.TopPostsByLikesAsync(from, to));

        [HttpGet("top-tags")]
        public async Task<IActionResult> TopTags()
            => Ok(await _report.TopTagsAsync());

        [HttpGet("stats")]
        public async Task<IActionResult> Stats()
            => Ok(await _report.GetStatsAsync());

        [HttpGet("user-stats/{userId}")]
        public async Task<IActionResult> UserStats([FromRoute] int userId,
            [FromQuery] DateTime? from, [FromQuery] DateTime? to)
            => Ok(await _report.GetUserReportAsync(userId, from, to));

        [HttpGet("export/{userId}")]
        public async Task<IActionResult> Export([FromRoute] int userId)
            => Ok(await _report.ExportUserDataAsync(userId));
    }
}
