using System;
using System.Collections.Generic;
using System.Text;

namespace Social_Network.Models
{
    internal class Comment
    {
        // Properties
        public int Id {  get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        // Foreign keys and navigation properties for Post and User
        public int PostId { get; set; }
        public Post? Post { get; set; } = null;
        public int UserId { get; set; }
        public User? User { get; set; } = null;
    }
}
