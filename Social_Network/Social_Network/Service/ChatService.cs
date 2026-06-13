using System.Net.Http.Json;
using Social_Network.Constants;
using Social_Network.Core.Models.DTOs;

namespace Social_Network.Service
{
    public class ChatService: IChatService
    {
        private readonly HttpClient _http;
        public ChatService(HttpClient http) => _http = http;

        public async Task<List<ChatDto>> GetUserChatsAsync(int userId)
        {
            try
            {
                return await _http.GetFromJsonAsync<List<ChatDto>>($"{ApiConfig.BaseUrl}/chat/user/{userId}") ?? new();
            }
            catch
            {
                return new();
            }
        }

        public async Task<ChatDto?> CreateOrGetChatAsync(int user1Id, int user2Id)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"{ApiConfig.BaseUrl}/chat", new {User1Id = user1Id, User2Id = user2Id});
                if (!response.IsSuccessStatusCode)
                {
                    return null;   
                }
                return await response.Content.ReadFromJsonAsync<ChatDto>();
            }
            catch
            {
                return null;
            }
        }

    }
}
