using System;
using System.Collections.Generic;
using System.Text;

namespace Social_Network.Models
{
    internal class Like
    {
        // Properties
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        // Foregin keys and navigation properties for Post and User
        public int PostId { get; set; }
        public Post? Post { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
