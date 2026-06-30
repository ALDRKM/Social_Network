using Microsoft.EntityFrameworkCore;
using Social_Network.API.Data;

namespace Social_Network.API.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _db;
        public ReportService(AppDbContext db) => _db = db;

        public async Task<List<TopPostDto>> TopPostsByLikesAsync(DateTime? from, DateTime? to, int take = 20)
        {
            var query = _db.Posts.Include(p => p.User).Include(p => p.Likes)
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag).AsQueryable();
            if (from.HasValue) query = query.Where(p => p.CreatedAt >= from.Value);
            if (to.HasValue) query = query.Where(p => p.CreatedAt <= to.Value);

            var list = await query
                .OrderByDescending(p => p.Likes.Count)
                .Take(take)
                .Select(p => new
                {
                    p.Id,
                    Login = p.User.Login,
                    p.Content,
                    Likes = p.Likes.Count,
                    p.CreatedAt,
                    Tags = p.PostTags.Select(t => t.Tag!.Name)
                })
                .ToListAsync();

            return list.Select(x => new TopPostDto(
                x.Id, x.Login,
                string.IsNullOrWhiteSpace(x.Content) ? string.Join(" ", x.Tags.Select(t => "#" + t)) : x.Content,
                x.Likes, x.CreatedAt)).ToList();
        }

        public async Task<List<TopTagDto>> TopTagsAsync(int take = 100) =>
            await _db.Tags
                .OrderByDescending(t => t.PostTags.Count)
                .Take(take)
                .Select(t => new TopTagDto(t.Name, t.PostTags.Count))
                .ToListAsync();

        public async Task<UserReportDto?> GetUserReportAsync(int userId, DateTime? from = null, DateTime? to = null)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;

            var postsQuery = _db.Posts.Where(p => p.UserId == userId);
            if (from.HasValue) postsQuery = postsQuery.Where(p => p.CreatedAt >= from.Value);
            if (to.HasValue) postsQuery = postsQuery.Where(p => p.CreatedAt <= to.Value);
            int posts = await postsQuery.CountAsync();

            var messagesQuery = _db.Messages.Where(m => m.SenderId == userId);
            if (from.HasValue) messagesQuery = messagesQuery.Where(m => m.SentAt >= from.Value);
            if (to.HasValue) messagesQuery = messagesQuery.Where(m => m.SentAt <= to.Value);
            int messagesSent = await messagesQuery.CountAsync();

            var likesReceivedQuery = _db.Likes.Where(l => l.Post!.UserId == userId);
            if (from.HasValue) likesReceivedQuery = likesReceivedQuery.Where(l => l.CreatedAt >= from.Value);
            if (to.HasValue) likesReceivedQuery = likesReceivedQuery.Where(l => l.CreatedAt <= to.Value);
            int likesReceived = await likesReceivedQuery.CountAsync();

            var likesGivenQuery = _db.Likes.Where(l => l.UserId == userId);
            if (from.HasValue) likesGivenQuery = likesGivenQuery.Where(l => l.CreatedAt >= from.Value);
            if (to.HasValue) likesGivenQuery = likesGivenQuery.Where(l => l.CreatedAt <= to.Value);
            int likesGiven = await likesGivenQuery.CountAsync();

            var commentsGivenQuery = _db.Comments.Where(c => c.UserId == userId);
            if (from.HasValue) commentsGivenQuery = commentsGivenQuery.Where(c => c.CreatedAt >= from.Value);
            if (to.HasValue) commentsGivenQuery = commentsGivenQuery.Where(c => c.CreatedAt <= to.Value);
            int commentsGiven = await commentsGivenQuery.CountAsync();

            var commentsReceivedQuery = _db.Comments.Where(c => c.Post!.UserId == userId);
            if (from.HasValue) commentsReceivedQuery = commentsReceivedQuery.Where(c => c.CreatedAt >= from.Value);
            if (to.HasValue) commentsReceivedQuery = commentsReceivedQuery.Where(c => c.CreatedAt <= to.Value);
            int commentsReceived = await commentsReceivedQuery.CountAsync();

            var followingQuery = _db.Subscriptions.Where(s => s.FollowerId == userId);
            if (from.HasValue) followingQuery = followingQuery.Where(s => s.CreatedAt >= from.Value);
            if (to.HasValue) followingQuery = followingQuery.Where(s => s.CreatedAt <= to.Value);
            int following = await followingQuery.CountAsync();

            var followersQuery = _db.Subscriptions.Where(s => s.FollowingId == userId);
            if (from.HasValue) followersQuery = followersQuery.Where(s => s.CreatedAt >= from.Value);
            if (to.HasValue) followersQuery = followersQuery.Where(s => s.CreatedAt <= to.Value);
            int followers = await followersQuery.CountAsync();

            var topTags = await TopTagsAsync();

            return new UserReportDto(user.Login, user.Email, posts, messagesSent,
                likesReceived, likesGiven, commentsReceived, commentsGiven,
                following, followers, topTags);
        }

        public async Task<StatsDto> GetStatsAsync() => new StatsDto(
            await _db.Users.CountAsync(),
            await _db.Posts.CountAsync(),
            await _db.Subscriptions.CountAsync(),
            await _db.Messages.CountAsync(),
            await _db.Comments.CountAsync(),
            await _db.Likes.CountAsync());

        public async Task<object> ExportUserDataAsync(int userId)
        {
            var user = await _db.Users
                .Include(u => u.Settings)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var posts = await _db.Posts
                .Where(p => p.UserId == userId)
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .Select(p => new { p.Id, p.Content, p.Type, p.CreatedAt, Tags = p.PostTags.Select(t => t.Tag!.Name) })
                .ToListAsync();

            var comments = await _db.Comments.Where(c => c.UserId == userId)
                .Select(c => new { c.Id, c.Text, c.PostId, c.CreatedAt }).ToListAsync();

            var likes = await _db.Likes.Where(l => l.UserId == userId)
                .Select(l => new { l.PostId, l.CreatedAt }).ToListAsync();

            var saved = await _db.SavedPosts.Where(s => s.UserId == userId)
                .Select(s => new { s.PostId, s.SavedAt }).ToListAsync();

            return new
            {
                User = user == null ? null : new { user.Id, user.Login, user.Email, user.Bio, user.CreatedAt },
                Posts = posts,
                Comments = comments,
                Likes = likes,
                Saved = saved
            };
        }
    }
}
