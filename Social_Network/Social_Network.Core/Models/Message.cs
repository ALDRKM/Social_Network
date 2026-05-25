using System;
using System.Collections.Generic;
using System.Text;

namespace Social_Network.Core.Models
{
    internal class Message
    {
        // Properties
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        // Foreign keys and navigation properties for Chat and User (Sender)
        public int ChatId { get; set; }
        public Chat? Chat { get; set; }
        public int SenderId { get; set; }
        public User? Sender { get; set; }
    }
}
