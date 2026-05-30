using Social_Network.Core.Models;
using Social_Network.API.Data;
using Microsoft.EntityFrameworkCore;


namespace Social_Network.API.Services
{
    public class UserService:IUserService
    {
        private readonly AppDbContext _db;
        public UserService(AppDbContext db) => _db = db;

        async public Task<User?> GetUserByIdAsync(int id) => await _db.Users.Include(user => user.Settings).FirstOrDefaultAsync(user => user.Id == id);   

        async public Task<User?> UpdateUserProfileAsync(int id, string login, string? bio, string? avatarURL)
        {
            var user = await _db.Users.FirstOrDefaultAsync(user => user.Id == id);
            if (user == null) return null;

            user.Login = login;
            user.Bio = bio;
            user.AvatarUrl = avatarURL;
            await _db.SaveChangesAsync();

            return user;
        }

        async public Task DeleteAccountAsync(int id)
        {

            var chats = await _db.Chats.Where(chat => chat.User2Id == id || chat.User1Id == id).ToListAsync();
            _db.Chats.RemoveRange(chats);

            var subs = await _db.Subscriptions.Where(sub => sub.FollowerId == id || sub.FollowingId == id).ToListAsync();
            _db.Subscriptions.RemoveRange(subs);

            var user = await _db.Users.FindAsync(id);

            if (user != null) _db.Users.Remove(user);

            await _db.SaveChangesAsync();

        }
        

    }
}
