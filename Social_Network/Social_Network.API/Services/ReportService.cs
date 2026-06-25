using Microsoft.EntityFrameworkCore;
using Social_Network.API.Data;

namespace Social_Network.API.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _db;
        public ReportService(AppDbContext db) => _db = db;

        // Самые популярные публикации по отметкам «нравится» за период
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

            // Для постов без описания показываем теги вместо пустой строки
            return list.Select(x => new TopPostDto(
                x.Id, x.Login,
                string.IsNullOrWhiteSpace(x.Content) ? string.Join(" ", x.Tags.Select(t => "#" + t)) : x.Content,
                x.Likes, x.CreatedAt)).ToList();
        }

        // Самые популярные теги по количеству использований
        public async Task<List<TopTagDto>> TopTagsAsync(int take = 20) =>
            await _db.Tags
                .OrderByDescending(t => t.PostTags.Count)
                .Take(take)
                .Select(t => new TopTagDto(t.Name, t.PostTags.Count))
                .ToListAsync();

        public async Task<StatsDto> GetStatsAsync() => new StatsDto(
            await _db.Users.CountAsync(),
            await _db.Posts.CountAsync(),
            await _db.Subscriptions.CountAsync(),
            await _db.Messages.CountAsync(),
            await _db.Comments.CountAsync(),
            await _db.Likes.CountAsync());

        // Экспорт всех данных пользователя
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
