using System.Net.Http.Headers;
using System.Net.Http.Json;
using Social_Network.Constants;

namespace Social_Network.Service
{
    public class ImageUploadService : IImageUploadService
    {
        private readonly HttpClient _http;
        public ImageUploadService(HttpClient http) => _http = http;

        private record UploadDto(string url);

        public async Task<string?> UploadAsync(Stream stream, string fileName)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                var sc = new StreamContent(stream);
                sc.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                content.Add(sc, "file", string.IsNullOrWhiteSpace(fileName) ? "image.jpg" : fileName);

                var resp = await _http.PostAsync($"{ApiConfig.BaseUrl}/upload", content);
                if (!resp.IsSuccessStatusCode) return null;

                var dto = await resp.Content.ReadFromJsonAsync<UploadDto>();
                return dto?.url;
            }
            catch
            {
                return null;
            }
        }
    }
}
