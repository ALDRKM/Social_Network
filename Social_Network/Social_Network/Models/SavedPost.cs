using System;
using System.Collections.Generic;
using System.Text;

namespace Social_Network.Models
{
    internal class SavedPost
    {
        // Property
        public int Id { get; set; }
        public DateTime SavedAt { get; set; } = DateTime.UtcNow;
        // Foreign key and navigation property for User and Post
        public int UserId { get; set; }
        public User? User { get; set; }
        public int PostId { get; set; }
        public Post? Post { get; set; }
    }
}
