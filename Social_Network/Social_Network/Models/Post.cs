using System;
using System.Collections.Generic;
using System.Text;

namespace Social_Network.Models
{
    internal class Post
    {
        // Properties
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }= DateTime.UtcNow;

        // Foreign key and navigation property for User
        public int UserId { get; set; }
        public User User { get; set; }

        // Navigation properties 
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<SavedPost> SavedPosts { get; set; } = new List<SavedPost>();
    }
}
