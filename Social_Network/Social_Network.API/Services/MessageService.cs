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

        public async Task<List<MessageDto>> GetByChatIdAsync(int chatId)
        {
            var messages = await _db.Messages.Include(m => m.Sender)
                .Where(m => m.ChatId == chatId).OrderBy(m => m.SentAt)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    ChatId = m.ChatId,
                    SenderId = m.SenderId,
                    Text = m.Text,
                    SentAt = m.SentAt,
                    IsRead = m.IsRead
                }).ToListAsync();

            await EnrichSubscriptionRequestMessagesAsync(messages);
            return messages;
        }

        private async Task EnrichSubscriptionRequestMessagesAsync(List<MessageDto> messages)
        {
            var pendingIds = new List<int>();
            foreach (var m in messages)
            {
                if (!m.Text.StartsWith("[SUB_REQ:", StringComparison.Ordinal)) continue;
                if (m.Text.Contains(":OK]", StringComparison.Ordinal) ||
                    m.Text.Contains(":DEN]", StringComparison.Ordinal)) continue;

                var idPart = m.Text.Split(']')[0];
                var idStr = idPart["[SUB_REQ:".Length..];
                if (int.TryParse(idStr, out var reqId))
                    pendingIds.Add(reqId);
            }

            if (pendingIds.Count == 0) return;

            var statuses = await _db.SubscriptionRequests
                .Where(r => pendingIds.Contains(r.Id) &&
                            r.Status != SubscriptionRequestStatus.Pending)
                .Select(r => new { r.Id, r.Status })
                .ToListAsync();

            foreach (var m in messages)
            {
                foreach (var st in statuses)
                {
                    var prefix = $"[SUB_REQ:{st.Id}]";
                    if (!m.Text.StartsWith(prefix, StringComparison.Ordinal)) continue;
                    var suffix = st.Status == SubscriptionRequestStatus.Approved ? "OK" : "DEN";
                    m.Text = $"[SUB_REQ:{st.Id}:{suffix}]{m.Text[prefix.Length..]}";
                }
            }
        }

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
