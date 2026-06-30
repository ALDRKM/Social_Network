using Microsoft.EntityFrameworkCore;
using Social_Network.API.Data;
using Social_Network.Core.Models;
using Social_Network.Core.Models.DTOs;

namespace Social_Network.API.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly AppDbContext _db;
        private readonly INotificationService _notify;

        public SubscriptionService(AppDbContext db, INotificationService notify)
        {
            _db = db;
            _notify = notify;
        }

        public async Task<FollowResultDto> FollowAsync(int followerId, int followingId)
        {
            if (followerId == followingId)
                return new FollowResultDto { Status = "already" };

            if (await IsFollowingAsync(followerId, followingId))
                return new FollowResultDto { Status = "already" };

            if (await HasPendingRequestAsync(followerId, followingId))
                return new FollowResultDto { Status = "requested" };

            var settings = await _db.UserSettings.FirstOrDefaultAsync(s => s.UserId == followingId);
            if (settings?.IsPrivateAccount == true)
            {
                var follower = await _db.Users.FindAsync(followerId);
                var request = new SubscriptionRequest
                {
                    FollowerId = followerId,
                    FollowingId = followingId
                };
                _db.SubscriptionRequests.Add(request);
                await _db.SaveChangesAsync();

                var login = follower?.Login ?? "пользователь";
                await _notify.SendSystemMessageAsync(followingId,
                    $"[SUB_REQ:{request.Id}]Пользователь @{login} хочет подписаться на вас");

                return new FollowResultDto { Status = "requested" };
            }

            _db.Subscriptions.Add(new Subscription
            {
                FollowerId = followerId,
                FollowingId = followingId
            });
            await _db.SaveChangesAsync();
            return new FollowResultDto { Status = "followed" };
        }

        public async Task UnFollowAsync(int followerId, int followingId)
        {
            var subs = await _db.Subscriptions.FirstOrDefaultAsync(sub =>
                sub.FollowerId == followerId && sub.FollowingId == followingId);
            if (subs != null)
            {
                _db.Subscriptions.Remove(subs);
                await _db.SaveChangesAsync();
            }

            var pending = await _db.SubscriptionRequests.FirstOrDefaultAsync(r =>
                r.FollowerId == followerId && r.FollowingId == followingId &&
                r.Status == SubscriptionRequestStatus.Pending);
            if (pending != null)
            {
                _db.SubscriptionRequests.Remove(pending);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> IsFollowingAsync(int followerId, int followingId) =>
            await _db.Subscriptions.AnyAsync(sub =>
                sub.FollowerId == followerId && sub.FollowingId == followingId);

        public async Task<bool> HasPendingRequestAsync(int followerId, int followingId) =>
            await _db.SubscriptionRequests.AnyAsync(r =>
                r.FollowerId == followerId && r.FollowingId == followingId &&
                r.Status == SubscriptionRequestStatus.Pending);

        public async Task ApproveRequestAsync(int requestId, int ownerId)
        {
            var request = await _db.SubscriptionRequests
                .Include(r => r.Follower)
                .Include(r => r.Following)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null || request.FollowingId != ownerId ||
                request.Status != SubscriptionRequestStatus.Pending)
                return;

            request.Status = SubscriptionRequestStatus.Approved;

            if (!await IsFollowingAsync(request.FollowerId, request.FollowingId))
            {
                _db.Subscriptions.Add(new Subscription
                {
                    FollowerId = request.FollowerId,
                    FollowingId = request.FollowingId
                });
            }

            await _db.SaveChangesAsync();

            await UpdateSubscriptionRequestMessageAsync(requestId, "OK");

            var ownerLogin = request.Following?.Login ?? "пользователь";
            await _notify.SendSystemMessageAsync(request.FollowerId,
                $"[SUB_OK]Пользователь @{ownerLogin} одобрил вашу заявку на подписку");
        }

        public async Task RejectRequestAsync(int requestId, int ownerId)
        {
            var request = await _db.SubscriptionRequests
                .Include(r => r.Follower)
                .Include(r => r.Following)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null || request.FollowingId != ownerId ||
                request.Status != SubscriptionRequestStatus.Pending)
                return;

            request.Status = SubscriptionRequestStatus.Rejected;
            await _db.SaveChangesAsync();

            await UpdateSubscriptionRequestMessageAsync(requestId, "DEN");

            var ownerLogin = request.Following?.Login ?? "пользователь";
            await _notify.SendSystemMessageAsync(request.FollowerId,
                $"[SUB_DEN]Пользователь @{ownerLogin} отклонил вашу заявку на подписку");
        }

        private async Task UpdateSubscriptionRequestMessageAsync(int requestId, string status)
        {
            var prefix = $"[SUB_REQ:{requestId}]";
            var message = await _db.Messages.FirstOrDefaultAsync(m => m.Text.StartsWith(prefix));
            if (message == null) return;

            var body = message.Text[prefix.Length..];
            message.Text = $"[SUB_REQ:{requestId}:{status}]{body}";
            await _db.SaveChangesAsync();
        }

        public async Task<List<int>> GetPendingFollowingIdsAsync(int followerId) =>
            await _db.SubscriptionRequests
                .Where(r => r.FollowerId == followerId && r.Status == SubscriptionRequestStatus.Pending)
                .Select(r => r.FollowingId)
                .ToListAsync();

        public async Task<int> GetFollowersCountAsync(int userId) =>
            await _db.Subscriptions.CountAsync(sub => sub.FollowingId == userId);

        public async Task<int> GetFollowingCountAsync(int userId) =>
            await _db.Subscriptions.CountAsync(sub => sub.FollowerId == userId);

        public async Task<List<int>> GetFollowingIdsAsync(int userId) =>
            await _db.Subscriptions
                .Where(sub => sub.FollowerId == userId)
                .Select(sub => sub.FollowingId)
                .ToListAsync();
    }
}
