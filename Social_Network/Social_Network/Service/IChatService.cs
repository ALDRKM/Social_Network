using Social_Network.Core.Models.DTOs;

namespace Social_Network.Service
{
    public interface IChatService
    {
        Task<List<ChatDto>> GetUserChatsAsync(int userId);
        Task<ChatDto?> CreateOrGetChatAsync(int user1Id, int user2Id);
    }
}
