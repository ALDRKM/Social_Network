using Microsoft.EntityFrameworkCore;
using Social_Network.API.Data;
using Social_Network.Core.Models;


namespace Social_Network.API.Services
{
    public class SubscriptionService: ISubscriptionService
    {
        private readonly AppDbContext _db;
        public SubscriptionService(AppDbContext db) => _db = db;

        public async Task FollowAsync(int followerId, int followingId)
        {
            bool already = await _db.Subscriptions.AnyAsync(sub => sub.FollowerId == followerId && sub.FollowingId == followingId);
            if (already) return;

            var subs = new Subscription()
            {
                FollowerId = followerId,
                FollowingId = followingId
            };


            _db.Subscriptions.Add(subs);

            await _db.SaveChangesAsync();
        }

        public async Task UnFollowAsync(int followerId, int followingId)
        {
            var unsubs= await _db.Subscriptions.FirstOrDefaultAsync(sub => sub.FollowerId == followerId && sub.FollowingId == followingId);

            if (unsubs != null)
            {
                _db.Subscriptions.Remove(unsubs);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> IsFollow(int followerId, int followingId) => await _db.Subscriptions.AnyAsync(sub => sub.FollowerId == followerId && sub.FollowingId == followingId);


    }
}
