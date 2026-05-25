using System;
using System.Collections.Generic;
using System.Text;

namespace Social_Network.Core.Models
{
    internal class Comment
    {
        // Properties
        public int Id {  get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Foreign keys and navigation properties for Post and User
        public int PostId { get; set; }
        public Post Post { get; set; } 
        public int UserId { get; set; }
        public User User { get; set; } 
    }
}
