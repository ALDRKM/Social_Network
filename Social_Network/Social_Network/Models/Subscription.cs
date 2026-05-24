using System;
using System.Collections.Generic;
using System.Text;

namespace Social_Network.Models
{
    internal class Subscription
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int FollewerId { get; set; }
        public User? Follower { get; set; }
        public int FollowingId { get; set; }
        public User? Following { get; set; }
    }
}
