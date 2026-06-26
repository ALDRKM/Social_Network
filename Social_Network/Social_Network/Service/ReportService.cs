using System.Net.Http.Json;
using Social_Network.Constants;

namespace Social_Network.Service
{
    public class ReportService : IReportService
    {
        private readonly HttpClient _http;
        public ReportService(HttpClient http) => _http = http;

        public async Task<List<TopPostDto>> TopPostsByLikesAsync(DateTime? from = null, DateTime? to = null)
        {
            try
            {
                var url = $"{ApiConfig.BaseUrl}/report/top-posts";
                var qs = new List<string>();
                if (from.HasValue) qs.Add($"from={from.Value:yyyy-MM-dd}");
                if (to.HasValue) qs.Add($"to={to.Value:yyyy-MM-dd}");
                if (qs.Count > 0) url += "?" + string.Join("&", qs);
                return await _http.GetFromJsonAsync<List<TopPostDto>>(url) ?? new();
            }
            catch
            {
                return new();
            }
        }

        public async Task<List<TopTagDto>> TopTagsAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<TopTagDto>>($"{ApiConfig.BaseUrl}/report/top-tags") ?? new();
            }
            catch
            {
                return new();
            }
        }

        public async Task<StatsDto?> GetStatsAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<StatsDto>($"{ApiConfig.BaseUrl}/report/stats");
            }
            catch
            {
                return null;
            }
        }

        public async Task<UserReportDto?> GetUserReportAsync(int userId)
        {
            try
            {
                return await _http.GetFromJsonAsync<UserReportDto>($"{ApiConfig.BaseUrl}/report/user-stats/{userId}");
            }
            catch
            {
                return null;
            }
        }

        // Возвращает JSON-строку с данными пользователя (для сохранения в файл)
        public async Task<string?> ExportUserDataAsync(int userId)
        {
            try
            {
                return await _http.GetStringAsync($"{ApiConfig.BaseUrl}/report/export/{userId}");
            }
            catch
            {
                return null;
            }
        }
    }
}
