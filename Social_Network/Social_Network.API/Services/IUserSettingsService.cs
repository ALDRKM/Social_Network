using Social_Network.Core.Models;

namespace Social_Network.API.Services
{
    public interface IUserSettingsService
    {
        Task<UserSettings?> GetByUserIdAsync(int userId);
        Task<UserSettings?> UpdateAsync(int userId, bool isPrivate, bool notificationsEnabled);
    }
}
