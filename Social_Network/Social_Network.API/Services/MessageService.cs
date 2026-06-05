using Social_Network.API.Data;
using Social_Network.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Social_Network.API.Services
{
    public class MessageService : IMessageService
    {
        private readonly AppDbContext _db;
        public MessageService(AppDbContext db) => _db = db;

        public async Task<List<Message>> GetByChatIdAsync(int chatId) =>
            await _db.Messages.Include(m => m.Sender).Where(m => m.ChatId == chatId).OrderBy(m => m.SentAt).ToListAsync();

        public async Task<Message> SendAsync(int chatId, int senderId, string text)
        {
            var message = new Message()
            {
                ChatId = chatId,
                SenderId = senderId,
                Text = text
            };
            _db.Messages.Add(message);
            await _db.SaveChangesAsync();
            return message;
        }

        public async Task MarkIsReadAsync(int messageId)
        {
            var message = await _db.Messages.FindAsync(messageId);
            if (message != null)
            {
                message.IsRead = true;
            }
            await _db.SaveChangesAsync();
        }

        public async Task<bool> RevokeMessageAsync(int messageId)
        {
            var message = await _db.Messages.FindAsync(messageId);
            if (message != null)
            {
                _db.Messages.Remove(message);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
