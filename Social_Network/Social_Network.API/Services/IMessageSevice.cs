using Social_Network.Core.Models.DTOs;

namespace Social_Network.API.Services
{
    public interface IMessageService
    {
        Task<List<MessageDto>> GetByChatIdAsync(int chatId);
        Task<MessageDto> SendAsync(int chatId, int senderId, string text);
        Task MarkIsReadAsync(int messageId);
        Task<bool> RevokeMessageAsync(int messageId);
    }
}
