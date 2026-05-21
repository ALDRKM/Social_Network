using System;
using System.Collections.Generic;
using System.Text;

namespace Social_Network.Models
{
    internal class User
    {
        // Properties
        public int Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? Gender { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsOnline { get; set; }

        // Navigation properties
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Subscription> Followers { get; set; } = new List<Subscription>();
        public ICollection<Subscription> Following { get; set; } = new List<Subscription>();
    }
}
