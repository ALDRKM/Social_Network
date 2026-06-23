using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Social_Network.Core.Models
{
    public class SavedPost
    {
        // Property
        public int Id { get; set; }
        public DateTime SavedAt { get; set; } = DateTime.UtcNow;
        // Foreign key and navigation property for User and Post
        public int UserId { get; set; }
        [JsonIgnore] public User? User { get; set; }
        public int PostId { get; set; }
        [JsonIgnore] public Post? Post { get; set; }
    }
}
