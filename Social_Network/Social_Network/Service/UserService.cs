using System.Net.Http.Json;
using Social_Network.Constants;
using Social_Network.Core.Models;

namespace Social_Network.Service
{
    public class UserService : IUserService
    {
        private readonly HttpClient _http;
        public UserService(HttpClient http) => _http = http;

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            try
            {
                return await _http.GetFromJsonAsync<User>($"{ApiConfig.BaseUrl}/user/{userId}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<User?> UpdateProfileAsync(int id, string login, string? bio, string? avatarUrl)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"{ApiConfig.BaseUrl}/user/{id}", new { login, bio, avatarUrl });

                if (!response.IsSuccessStatusCode) return null;

                return await response.Content.ReadFromJsonAsync<User>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DeleteAccountAsync(int id)
        {
            try
            {
                return (await _http.DeleteAsync($"{ApiConfig.BaseUrl}/user/{id}")).IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
