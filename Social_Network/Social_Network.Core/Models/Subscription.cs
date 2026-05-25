using System;
using System.Collections.Generic;
using System.Text;

namespace Social_Network.Core.Models
{
    internal class Subscription
    {
        // Properties
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Foreign keys and navigation properties for User (Follower and Following)
        public int FollewerId { get; set; }
        public User? Follower { get; set; }
        public int FollowingId { get; set; }
        public User? Following { get; set; }
    }
}
