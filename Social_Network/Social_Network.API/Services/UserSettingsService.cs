using Social_Network.Core.Models;
using Microsoft.EntityFrameworkCore;
using Social_Network.API.Data;

namespace Social_Network.API.Services
{
    public class UserSettingsService: IUserSettingsService
    {
        private readonly AppDbContext _db;
        public UserSettingsService(AppDbContext db) => _db = db;

        public async Task<UserSettings?> GetByUserIdAsync(int userId) =>
            await _db.UserSettings.Include(x => x.User).FirstOrDefaultAsync(set => set.UserId == userId);

        public async Task<UserSettings?> UpdateAsync(int userId, bool isPrivate, bool notificationsEnabled)
        {
            var userset = await _db.UserSettings.FirstOrDefaultAsync(set => set.UserId == userId);
            if (userset == null) return null;

            userset.IsPrivateAccount = isPrivate;
            userset.NotificationsEnabled = notificationsEnabled;

            
            await _db.SaveChangesAsync();
            return userset;
        }
    }
}
