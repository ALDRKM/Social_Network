using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Social_Network.Core.Models
{
    public class Like
    {
        // Properties
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Foregin keys and navigation properties for Post and User
        public int PostId { get; set; }
        [JsonIgnore] public Post? Post { get; set; }
        public int UserId { get; set; }
        [JsonIgnore] public User? User { get; set; }
    }
}
