using Social_Network.API.Data;
using Social_Network.Core.Models;
using Social_Network.Core.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Social_Network.API.Services
{
    public class MessageService : IMessageService
    {
        private readonly AppDbContext _db;
        public MessageService(AppDbContext db) => _db = db;

        public async Task<List<MessageDto>> GetByChatIdAsync(int chatId) =>
            await _db.Messages.Include(m => m.Sender).Where(m => m.ChatId == chatId).OrderBy(m => m.SentAt).Select(m => new MessageDto
            {
                Id = m.Id,
                ChatId = m.ChatId,
                SenderId = m.SenderId,
                Text = m.Text,
                SentAt = m.SentAt,
                IsRead = m.IsRead
            }).ToListAsync();

        public async Task<MessageDto> SendAsync(int chatId, int senderId, string text)
        {
            var message = new Message()
            {
                ChatId = chatId,
                SenderId = senderId,
                Text = text
            };
            _db.Messages.Add(message);
            await _db.SaveChangesAsync();
            return new MessageDto
            {
                Id = message.Id,
                ChatId = message.ChatId,
                SenderId = message.SenderId,
                Text = message.Text,
                SentAt = message.SentAt,
                IsRead = message.IsRead
            };
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
