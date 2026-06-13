using Social_Network.Constants;
using Social_Network.Core.Models.DTOs;
using System.Net.Http.Json;

namespace Social_Network.Service
{
    public class MessageService: IMessageService
    {
        private readonly HttpClient _http;
        public MessageService(HttpClient http) => _http = http;

        public async Task<List<MessageDto>> GetByChatIdAsync(int chatId)
        {
            try
            {
                return await _http.GetFromJsonAsync<List<MessageDto>>($"{ApiConfig.BaseUrl}/message/{chatId}")?? new();
            }
            catch
            {
                return new();
            }
        }

        public async Task<MessageDto?> SendAsync(int chatId, int senderId, string text)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"{ApiConfig.BaseUrl}/message", new { SenderId = senderId, Text = text });
                if (!response.IsSuccessStatusCode) return null;

                return await response.Content.ReadFromJsonAsync<MessageDto>();
            }
            catch
            {
                return null;
            }
        }

        public async Task MarksAsReadAsync(int messageId, int userId, int chatId)
        {
            try
            {
                await _http.PutAsync($"{ApiConfig.BaseUrl}/message/{messageId}/read?userId={userId}&chatId={chatId}", null);
            }
            catch
            {

            }
        }
    }
}
