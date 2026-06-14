using Social_Network.Constants;
using System.Net.Http.Json;

namespace Social_Network.Service
{
    public class LikeService:ILikeService
    {
        private readonly HttpClient _http;
        public LikeService(HttpClient http) => _http = http;

        public async Task LikeAsync(int userId, int postId)
        {
            try
            {
                await _http.PostAsync($"{ApiConfig.BaseUrl}/like/{userId}/{postId}",null);
            }
            catch
            {

            }
        }
        public async Task UnlikeAsync(int userId, int postId)
        {
            try
            {
                await _http.DeleteAsync($"{ApiConfig.BaseUrl}/like/{userId}/{postId}");
            }
            catch
            {

            }
        }

        public async Task<bool> IsLikeAsync(int userId, int postId)
        {
            try
            {
                return await _http.GetFromJsonAsync<bool>($"{ApiConfig.BaseUrl}/like/isliked/{userId}/{postId}");
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> GetLikeCountAsync(int postId)
        {
            try
            {
                return await _http.GetFromJsonAsync<int>($"{ApiConfig.BaseUrl}/like/count/{postId}");
            }
            catch
            {
                return 0;
            }
        }

    }
}
