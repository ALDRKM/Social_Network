using System.Net.Http.Json;
using Social_Network.Constants;
using Social_Network.Core.Models;

namespace Social_Network.Service
{
    public class UserSettingsService : IUserSettingsService
    {
        private readonly HttpClient _http;
        public UserSettingsService(HttpClient http) => _http = http;

        public async Task<UserSettings?> GetAsync(int userId)
        {
            try
            {
                return await _http.GetFromJsonAsync<UserSettings>($"{ApiConfig.BaseUrl}/usersettings/{userId}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateAsync(int userId, bool isPrivate, bool notificationsEnabled)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"{ApiConfig.BaseUrl}/usersettings/{userId}",
                    new { IsPrivate = isPrivate, NotificationEnabled = notificationsEnabled });
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
