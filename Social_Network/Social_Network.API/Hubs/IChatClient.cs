using Social_Network.Core.Models.DTOs;

namespace Social_Network.API.Hubs
{
    public interface IChatClient
    {
        Task MessageReceivedAsync(MessageDto message);
        Task MessageDeletedAsync(int messageId);
        Task MessageReadAsync(int messageId, int readByUserId);
    }
}
