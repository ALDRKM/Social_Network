using System.Net.Http.Json;
using Social_Network.Constants;
using Social_Network.Core.Models;

namespace Social_Network.Service
{
    public class CommentService: ICommentService
    {
        private readonly HttpClient _http;
        public CommentService(HttpClient http) => _http = http;

        public async Task<List<Comment>> GetCommentsAsync(int postId)
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Comment>>($"{ApiConfig.BaseUrl}/comment/post/{postId}")?? new();
            }
            catch
            {
                return new();
            }
        }

        public async Task<Comment?> CreateCommentAsync(int userId, int postId, string content, int? parentCommentId = null)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"{ApiConfig.BaseUrl}/comment",
                    new { UserId = userId, PostId = postId, Content = content, ParentCommentId = parentCommentId });

                if (!response.IsSuccessStatusCode) return null;

                return await response.Content.ReadFromJsonAsync<Comment>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> EditCommentAsync(int userId, int commentId, string content)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"{ApiConfig.BaseUrl}/comment/{commentId}",
                    new { UserId = userId, Content = content });
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteCommentAsync(int userId, int commentId)
        {
            try
            {
                return (await _http.DeleteAsync($"{ApiConfig.BaseUrl}/comment/{userId}/{commentId}")).IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Comment>> GetUserCommentsAsync(int userId)
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Comment>>($"{ApiConfig.BaseUrl}/comment/user/{userId}") ?? new();
            }
            catch
            {
                return new();
            }
        }
    }
}
