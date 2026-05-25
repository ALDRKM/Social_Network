using System;
using System.Collections.Generic;
using System.Text;

namespace Social_Network.Core.Models
{
    internal class Chat
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Foreign keys and navigation properties for User (User1 and User2)
        public int User1Id { get; set; }
        public User? User1 { get; set; }
        public int User2Id { get; set; }
        public User? User2 { get; set; }
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
