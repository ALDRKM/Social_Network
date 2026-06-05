using Social_Network.Core.Models;


namespace Social_Network.API.Services
{
    public interface IMessageSevice
    {
        Task<List<Message>> GetByChatIdasync(int chatId);
        Task<Message> SendAsync(int chatId, int senderId, int text);
        Task MarkIsReadAsync(int messageId);
    }
}
