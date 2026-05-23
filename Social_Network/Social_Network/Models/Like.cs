using System;
using System.Collections.Generic;
using System.Text;

namespace Social_Network.Models
{
    internal class Like
    {
        // Properties
        public int Id { get; set; }
        // Foregin keys and navigation properties for Post and User
        public int PostId { get; set; }
        public Post? Post { get; set; } = null;
        public int UserId { get; set; }
        public User? User { get; set; } = null;
    }
}
