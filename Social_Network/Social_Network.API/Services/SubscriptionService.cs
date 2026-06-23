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

        public async Task<bool> IsFollowingAsync(int followerId, int followingId) => await _db.Subscriptions.AnyAsync(sub => sub.FollowerId == followerId && sub.FollowingId == followingId);

        // Сколько подписчиков у пользователя
        public async Task<int> GetFollowersCountAsync(int userId) =>
            await _db.Subscriptions.CountAsync(sub => sub.FollowingId == userId);

        // На скольких подписан пользователь
        public async Task<int> GetFollowingCountAsync(int userId) =>
            await _db.Subscriptions.CountAsync(sub => sub.FollowerId == userId);

        // Id пользователей, на которых подписан данный пользователь
        public async Task<List<int>> GetFollowingIdsAsync(int userId) =>
            await _db.Subscriptions
                .Where(sub => sub.FollowerId == userId)
                .Select(sub => sub.FollowingId)
                .ToListAsync();
    }
}
