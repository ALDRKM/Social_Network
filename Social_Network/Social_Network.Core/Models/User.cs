using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Social_Network.Core.Models
{
    public class User
    {
        // Properties
        public int Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public string PasswordHash { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsOnline { get; set; }

        // Navigation properties (не сериализуются — нужны только на сервере)
        [JsonIgnore] public UserSettings? Settings { get; set; }
        [JsonIgnore] public ICollection<Post> Posts { get; set; } = new List<Post>();
        [JsonIgnore] public ICollection<Subscription> Followers { get; set; } = new List<Subscription>();
        [JsonIgnore] public ICollection<Subscription> Following { get; set; } = new List<Subscription>();
        [JsonIgnore] public ICollection<Like> Likes { get; set; } = new List<Like>();
        [JsonIgnore] public ICollection<SavedPost> SavedPosts { get; set; } = new List<SavedPost>();
        [JsonIgnore] public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
