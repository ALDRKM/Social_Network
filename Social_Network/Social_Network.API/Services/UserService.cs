using Social_Network.Core.Models;
using Social_Network.API.Data;
using Microsoft.EntityFrameworkCore;


namespace Social_Network.API.Services
{
    public class UserService:IUserService
    {
        private readonly AppDbContext _db;
        public UserService(AppDbContext db) => _db = db;

        public async Task<User?> GetUserByIdAsync(int id) => await _db.Users.Include(user => user.Settings).FirstOrDefaultAsync(user => user.Id == id);   

        public async Task<User?> UpdateUserProfileAsync(int id, string login, string? bio, string? avatarURL)
        {
            var user = await _db.Users.FirstOrDefaultAsync(user => user.Id == id);
            if (user == null) return null;

            user.Login = login;
            user.Bio = bio;
            user.AvatarUrl = avatarURL;
            await _db.SaveChangesAsync();

            return user;
        }

        public async Task<bool> DeleteAccountAsync(int id)
        {

            var chats = await _db.Chats.Where(chat => chat.User2Id == id || chat.User1Id == id).ToListAsync();
            _db.Chats.RemoveRange(chats);

            var subs = await _db.Subscriptions.Where(sub => sub.FollowerId == id || sub.FollowingId == id).ToListAsync();
            _db.Subscriptions.RemoveRange(subs);

            var mes = await _db.Messages.Where(mes => mes.SenderId == id).ToListAsync();
            _db.Messages.RemoveRange(mes);

            var likes = await _db.Likes.Where(like => like.UserId == id).ToListAsync();
            _db.Likes.RemoveRange(likes);

            var saves = await _db.SavedPosts.Where(sv => sv.UserId == id).ToListAsync();
            _db.SavedPosts.RemoveRange(saves);

            var posts = await _db.Posts.Where(p => p.UserId == id).ToListAsync();
            _db.Posts.RemoveRange(posts);

            var comment = await _db.Comments.Where(c => c.UserId == id).ToListAsync();
            _db.Comments.RemoveRange(comment);

            var settings = await _db.UserSettings.Where(uset => uset.UserId == id).ToListAsync();
            _db.UserSettings.RemoveRange(settings);

            var user = await _db.Users.FindAsync(id);

            if (user == null) return false; 
                
            
            _db.Users.Remove(user);

            await _db.SaveChangesAsync();

            return true;
        }
        

        public async Task<List<User>> SearchUsersAsync(string query)
        {
            return await _db.Users.Where(u => u.Login.ToLower().Contains(query.ToLower())).Take(30).ToListAsync();
        }

    }
}
