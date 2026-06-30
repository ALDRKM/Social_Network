namespace Social_Network.Core.Models
{
    public class SubscriptionRequest
    {
        public int Id { get; set; }
        public int FollowerId { get; set; }
        public User? Follower { get; set; }
        public int FollowingId { get; set; }
        public User? Following { get; set; }
        public SubscriptionRequestStatus Status { get; set; } = SubscriptionRequestStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
