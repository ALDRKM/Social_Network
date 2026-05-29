using Social_Network.Core.Models;
using Social_Network.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;


namespace Social_Network.API.Services
{
    public class AuthService: IAuthService
    {
        private readonly AppDbContext _db;
        public AuthService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<User?> RegisterAsync(string login, string email, string password)
        {
            bool exists = await _db.Users.AnyAsync(user => user.Email == email);

            if(exists)
                return null;

            var user = new User
            {
                Login = login,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var settings = new UserSettings() { UserId = user.Id };
            _db.UserSettings.Add(settings);
            await _db.SaveChangesAsync();
            
            return user;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(user => user.Email == email);
            if (user == null) return null;

            bool valid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            return valid ? user : null;
        }
    }
}
