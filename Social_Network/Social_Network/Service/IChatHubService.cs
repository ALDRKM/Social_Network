using Social_Network.Core.Models.DTOs;

namespace Social_Network.Service
{
    public interface IChatHubService
    {
        Task StartAsync();
        Task StopAsync();
        Task JoinChatAsync(int chatId, int userId);
        Task LeaveChatAsync(int chatId, int userId);
        
        event Action<MessageDto>? MessageReceived;
        event Action<int>? MessageDeleted;
        event Action<int, int>? MessageRead;
    }
}
