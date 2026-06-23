using Social_Network.Constants;
using System.Net.Http.Json;


namespace Social_Network.Service
{
    public class SubscriptionService: ISubscriptionService
    {
        private readonly HttpClient _http;
        public SubscriptionService(HttpClient http) => _http = http;

        public async Task FollowAsync(int followerId, int followingId)
        {
            try
            {
                await _http.PostAsJsonAsync($"{ApiConfig.BaseUrl}/subscription/follow", new {FollowerId = followerId, FollowingId = followingId});
            }
            catch { }
        }

        public async Task UnfollowAsync(int followerId, int followingId)
        {
            try
            {
                await _http.DeleteAsync($"{ApiConfig.BaseUrl}/subscription/unfollow/{followerId}/{followingId}");
            }
            catch { }
        }
        public async Task<bool> IsFollowingAsync(int followerId, int followingId)
        {
            try
            {
                return await _http.GetFromJsonAsync<bool>($"{ApiConfig.BaseUrl}/subscription/isfollowing/{followerId}/{followingId}");
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> GetFollowersCountAsync(int userId)
        {
            try { return await _http.GetFromJsonAsync<int>($"{ApiConfig.BaseUrl}/subscription/followers/count/{userId}"); }
            catch { return 0; }
        }

        public async Task<int> GetFollowingCountAsync(int userId)
        {
            try { return await _http.GetFromJsonAsync<int>($"{ApiConfig.BaseUrl}/subscription/following/count/{userId}"); }
            catch { return 0; }
        }

        public async Task<List<int>> GetFollowingIdsAsync(int userId)
        {
            try { return await _http.GetFromJsonAsync<List<int>>($"{ApiConfig.BaseUrl}/subscription/following/{userId}") ?? new(); }
            catch { return new(); }
        }
    }
}
