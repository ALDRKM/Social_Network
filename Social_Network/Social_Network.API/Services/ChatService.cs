using Social_Network.Core.Models;
using Microsoft.EntityFrameworkCore;
using Social_Network.API.Data;

namespace Social_Network.API.Services
{
    public class ChatService: IChatService
    {
        private readonly AppDbContext _db;
        public ChatService(AppDbContext db) => _db = db;

        public async Task<List<Chat>> GetUserChatsAsync(int userId) => await _db.Chats
            .Include(chat => chat.User2)
            .Include(chat => chat.User1)
            .Where(chat => chat.User1Id == userId || chat.User2Id == userId).ToListAsync();

        public async Task<Chat> CreateOrGetChatUserAsync(int user1Id, int user2Id)
        {
            var already = await _db.Chats.FirstOrDefaultAsync(c => (c.User1Id == user1Id && c.User2Id == user2Id) ||
            (c.User2Id == user1Id && c.User1Id == user2Id));
            if(already == null)
            {
                var chat = new Chat()
                {
                    User1Id = user1Id,
                    User2Id = user2Id
                };
                _db.Chats.Add(chat);
                await _db.SaveChangesAsync();
                return chat;
            }
            return already;
        }

        public async Task<bool> DeleteChatAsync(int chatId)
        {
            var chat = await _db.Chats.FindAsync(chatId);
            if (chat == null) return false;

            _db.Chats.Remove(chat);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserHasAccessToChatAsync(int userId, int chatId) => await _db.Chats.AnyAsync(c => c.Id == chatId && (c.User1Id == userId || c.User2Id == userId));
    }
}
