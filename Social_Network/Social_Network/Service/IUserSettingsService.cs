using Social_Network.Core.Models;

namespace Social_Network.Service
{
    public interface IUserSettingsService
    {
        Task<UserSettings?> GetAsync(int userId);
        Task<bool> UpdateAsync(int userId, bool isPrivate, bool notificationsEnabled);
    }
}
