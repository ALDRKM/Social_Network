using Social_Network.Core.Models.DTOs;

namespace Social_Network.Service
{
    public interface IMessageService
    {
        Task<List<MessageDto>> GetByChatIdAsync(int chatId);
        Task<MessageDto?> SendAsync(int chatId, int senderId, string text);
        Task MarksAsReadAsync(int messageId, int userId, int chatId );
    }
}
