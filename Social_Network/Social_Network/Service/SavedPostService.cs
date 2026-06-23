using System.Net.Http.Json;
using Social_Network.Constants;
using Social_Network.Core.Models;

namespace Social_Network.Service
{
    public class SavedPostService : ISavedPostService
    {
        private readonly HttpClient _http;
        public SavedPostService(HttpClient http) => _http = http;

        public async Task SaveAsync(int userId, int postId)
        {
            try { await _http.PostAsync($"{ApiConfig.BaseUrl}/savedpost/{userId}/{postId}", null); }
            catch { }
        }

        public async Task UnsaveAsync(int userId, int postId)
        {
            try { await _http.DeleteAsync($"{ApiConfig.BaseUrl}/savedpost/{userId}/{postId}"); }
            catch { }
        }

        public async Task<bool> IsSavedAsync(int userId, int postId)
        {
            try
            {
                return await _http.GetFromJsonAsync<bool>($"{ApiConfig.BaseUrl}/savedpost/issaved/{userId}/{postId}");
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Post>> GetSavedAsync(int userId)
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Post>>($"{ApiConfig.BaseUrl}/savedpost/user/{userId}") ?? new();
            }
            catch
            {
                return new();
            }
        }

        public async Task<List<int>> GetSavedIdsAsync(int userId)
        {
            try
            {
                return await _http.GetFromJsonAsync<List<int>>($"{ApiConfig.BaseUrl}/savedpost/ids/{userId}") ?? new();
            }
            catch
            {
                return new();
            }
        }
    }
}
