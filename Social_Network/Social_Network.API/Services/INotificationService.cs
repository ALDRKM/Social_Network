namespace Social_Network.API.Services
{
    public interface INotificationService
    {
        Task EnsureSystemChatAsync(int userId);
        Task SendSystemMessageAsync(int userId, string text);
    }
}
