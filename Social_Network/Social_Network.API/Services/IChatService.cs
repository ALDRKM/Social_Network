using Social_Network.Core.Models;


namespace Social_Network.API.Services
{
    public interface IChatService
    {
        Task<List<Chat>> GetUserChatsAsync(int userId);
        Task<Chat> CreateOrGetChatUserAsync(int user1Id, int user2Id);
        Task<bool> DeleteChatAsync(int chatId);
        Task<bool> UserHasAccessToChatAsync(int userId, int chatId);
    }
}
