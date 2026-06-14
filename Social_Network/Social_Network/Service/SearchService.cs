using System.Net.Http.Json;
using Social_Network.Constants;
using Social_Network.Core.Models;

namespace Social_Network.Service
{
    public class SearchService : ISearchService
    {
        private readonly HttpClient _http;
        public SearchService(HttpClient http) => _http = http;

        public async Task<List<User>> SearchUsersAsync(string query)
        {
            try
            {
                return await _http.GetFromJsonAsync<List<User>>($"{ApiConfig.BaseUrl}/user/search?query={Uri.EscapeDataString(query)}") ?? new();
            }
            catch
            {
                return new();
            }
        }

        public async Task<List<Post>> SearchPostsAsync(string query)
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Post>>($"{ApiConfig.BaseUrl}/post/search?query={Uri.EscapeDataString(query)}") ?? new();
            }
            catch
            {
                return new();
            }
        }
    }
}
