using System.Net.Http.Json;
using Social_Network.Constants;
using Social_Network.Core.Models;

namespace Social_Network.Service
{
    public class PostService: IPostService
    {
        private readonly HttpClient _http;
        public PostService(HttpClient http) => _http = http;


        public async Task<Post?> GetByIdAsync(int postId)
        {
            try
            {
                return await _http.GetFromJsonAsync<Post?>($"{ApiConfig.BaseUrl}/post/{postId}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Post>> GetFeedAsync(int userId)
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Post>>($"{ApiConfig.BaseUrl}/post/feed/{userId}") ?? new();
            }
            catch
            {
                return new();
            }
        }

        public async Task<List<Post>> GetUserPostsAsync(int userId, PostType? type = null)
        {
            try
            {
                var url = $"{ApiConfig.BaseUrl}/post/user/{userId}";
                if (type.HasValue) url += $"?type={(int)type.Value}";
                return await _http.GetFromJsonAsync<List<Post>>(url) ?? new();
            }
            catch
            {
                return new();
            }
        }

        public async Task<Post?> CreatePostAsync(int userId, string content, PostType type,
            string? imageUrl = null, List<string>? tags = null, List<int>? mentionUserIds = null)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"{ApiConfig.BaseUrl}/post", new
                {
                    UserId = userId,
                    Content = content,
                    Type = type,
                    ImageUrl = imageUrl,
                    Tags = tags,
                    MentionUserIds = mentionUserIds
                });

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

        public async Task<List<Post>> SearchPostsAsync(string query)
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Post>>(
                    $"{ApiConfig.BaseUrl}/post/search?query={Uri.EscapeDataString(query)}") ?? new();
            }
            catch
            {
                return new();
            }
        }

        public async Task<List<Post>> SearchByTagAsync(string tag)
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Post>>(
                    $"{ApiConfig.BaseUrl}/post/tag/{Uri.EscapeDataString(tag.TrimStart('#'))}") ?? new();
            }
            catch
            {
                return new();
            }
        }

        public async Task<List<string>> SearchTagNamesAsync(string query)
        {
            try
            {
                return await _http.GetFromJsonAsync<List<string>>(
                    $"{ApiConfig.BaseUrl}/post/tags/search?query={Uri.EscapeDataString(query)}") ?? new();
            }
            catch
            {
                return new();
            }
        }
    }
}
