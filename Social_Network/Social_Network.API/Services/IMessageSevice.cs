using Social_Network.Core.Models;

namespace Social_Network.API.Services
{
    public interface IMessageService
    {
        Task<List<Message>> GetByChatIdAsync(int chatId);
        Task<Message> SendAsync(int chatId, int senderId, string text);
        Task MarkIsReadAsync(int messageId);
        Task<bool> RevokeMessageAsync(int messageId);
    }
}
