using Social_Network.Core.Models;
using Social_Network.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Social_Network.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly INotificationService _notify;

        public AuthService(AppDbContext db, INotificationService notify)
        {
            _db = db;
            _notify = notify;
        }

        public async Task<User?> RegisterAsync(string login, string email, string password)
        {
            bool exists = await _db.Users.AnyAsync(user => user.Email == email);
            if (exists)
                return null;

            var user = new User
            {
                Login = login,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var settings = new UserSettings { UserId = user.Id };
            _db.UserSettings.Add(settings);
            await _db.SaveChangesAsync();

            await _notify.EnsureSystemChatAsync(user.Id);
            return user;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(user => user.Email == email);
            if (user == null || user.IsSystemAccount) return null;

            bool valid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!valid) return null;

            user.IsOnline = true;
            await _db.SaveChangesAsync();
            await _notify.EnsureSystemChatAsync(user.Id);
            return user;
        }

        public async Task SetOnlineAsync(int userId, bool online)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return;
            user.IsOnline = online;
            if (!online) user.LastSeen = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }
}
