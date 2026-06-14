using System.Net.Http.Json;
using Social_Network.Constants;
using Social_Network.Core.Models;

namespace Social_Network.Service
{
    public class PostService: IPostService
    {
        private readonly HttpClient _http;
        public PostService(HttpClient http) => _http = http;

        public async Task<List<Post>> GetFeedAsync(int userId)
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Post>>($"{ApiConfig.BaseUrl}/post/feed/{userId}")?? new();
            }
            catch
            {
                return new();
            }
        }

        public async Task<List<Post>> GetUserPostsAsync(int userId)
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Post>>($"{ApiConfig.BaseUrl}/post/user/{userId}")?? new();
            }
            catch
            {
                return new();
            }
        }

        public async Task<Post?> CreatePostAsync(int userId, string content, string? imageUrl)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"{ApiConfig.BaseUrl}/post",new { UserId = userId, Content = content, ImageUrl = imageUrl });

                if (!response.IsSuccessStatusCode) return null;

                return await response.Content.ReadFromJsonAsync<Post>();
            }
            catch
            {
                return null;
            }
        }

        public async Task DeletePostAsync(int userId, int postId)
        {
            try
            {
                await _http.DeleteAsync($"{ApiConfig.BaseUrl}/post/{userId}/{postId}");
            }
            catch
            {

            }
        }
    }
}
