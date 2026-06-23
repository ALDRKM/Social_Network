using Microsoft.AspNetCore.Mvc;

namespace Social_Network.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public UploadController(IWebHostEnvironment env) => _env = env;

        // Принимает файл и сохраняет в wwwroot/uploads, возвращает относительный URL
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("Пустой файл");

            var dir = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads");
            Directory.CreateDirectory(dir);

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext)) ext = ".jpg";
            var name = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(dir, name);

            using (var fs = System.IO.File.Create(fullPath))
                await file.CopyToAsync(fs);

            return Ok(new { url = $"/uploads/{name}" });
        }
    }
}
