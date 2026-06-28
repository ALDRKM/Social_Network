using Microsoft.EntityFrameworkCore;
using Social_Network.API.Data;
using Social_Network.Core.Models;

namespace Social_Network.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _db;

        public NotificationService(AppDbContext db) => _db = db;

        private async Task<int> GetSystemUserIdAsync()
        {
            var id = await _db.Users.Where(u => u.IsSystemAccount).Select(u => u.Id).FirstOrDefaultAsync();
            if (id == 0)
                throw new InvalidOperationException("System user is not configured");
            return id;
        }

        public async Task EnsureSystemChatAsync(int userId)
        {
            var systemId = await GetSystemUserIdAsync();
            if (userId == systemId) return;

            var exists = await _db.Chats.AnyAsync(c =>
                (c.User1Id == userId && c.User2Id == systemId) ||
                (c.User2Id == userId && c.User1Id == systemId));
            if (exists) return;

            _db.Chats.Add(new Chat { User1Id = userId, User2Id = systemId });
            await _db.SaveChangesAsync();
        }

        public async Task SendSystemMessageAsync(int userId, string text)
        {
            var systemId = await GetSystemUserIdAsync();
            if (userId == systemId) return;

            var chat = await _db.Chats.FirstOrDefaultAsync(c =>
                (c.User1Id == userId && c.User2Id == systemId) ||
                (c.User2Id == userId && c.User1Id == systemId));

            if (chat == null)
            {
                chat = new Chat { User1Id = userId, User2Id = systemId };
                _db.Chats.Add(chat);
                await _db.SaveChangesAsync();
            }

            _db.Messages.Add(new Message
            {
                ChatId = chat.Id,
                SenderId = systemId,
                Text = text
            });
            await _db.SaveChangesAsync();
        }
    }
}
