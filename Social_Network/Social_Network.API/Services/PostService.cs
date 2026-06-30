using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Social_Network.API.Data;
using Social_Network.Core.Models;

namespace Social_Network.API.Services
{
    public class PostService : IPostService
    {
        private readonly AppDbContext _db;
        private readonly INotificationService _notify;

        public PostService(AppDbContext db, INotificationService notify)
        {
            _db = db;
            _notify = notify;
        }

        private static readonly Regex HashtagRegex = new(@"#(\w+)", RegexOptions.Compiled);

        private IQueryable<Post> PostsWithIncludes() =>
            _db.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.Mentions).ThenInclude(m => m.MentionedUser);

        private static List<Post> SortFeedPosts(List<Post> posts) =>
            posts.OrderByDescending(p => p.CreatedAt).ToList();

        public async Task<Post?> GetByIdAsync(int postId) =>
            await PostsWithIncludes().FirstOrDefaultAsync(p => p.Id == postId);

        public async Task<List<Post>> GetFeedAsync(int userId, string mode = "all")
        {
            var query = PostsWithIncludes();

            if (mode == "following")
            {
                var followingIds = await _db.Subscriptions
                    .Where(s => s.FollowerId == userId)
                    .Select(s => s.FollowingId)
                    .ToListAsync();
                followingIds.Add(userId);
                query = query.Where(p => followingIds.Contains(p.UserId));
            }

            var posts = await query.ToListAsync();
            return SortFeedPosts(posts);
        }

        public async Task<List<Post>> GetUserPostsAsync(int userId, PostType? type = null)
        {
            var query = PostsWithIncludes().Where(post => post.UserId == userId);
            if (type.HasValue)
                query = query.Where(post => post.Type == type.Value);

            return await query.OrderByDescending(post => post.CreatedAt).ToListAsync();
        }

        public async Task<Post> CreatePostAsync(int userId, string content, PostType type,
            string? imageUrl = null, List<string>? tags = null, List<int>? mentionUserIds = null)
        {
            content ??= string.Empty;

            var tagNames = HashtagRegex.Matches(content)
                .Select(m => m.Groups[1].Value)
                .Concat(tags ?? Enumerable.Empty<string>())
                .Select(t => t.TrimStart('#').Trim().ToLowerInvariant())
                .Where(t => t.Length > 0)
                .Distinct()
                .ToList();

            var cleanContent = HashtagRegex.Replace(content, string.Empty).Trim();
            while (cleanContent.Contains("  ")) cleanContent = cleanContent.Replace("  ", " ");

            var post = new Post
            {
                UserId = userId,
                Content = cleanContent,
                Type = type,
                ImageUrl = imageUrl
            };

            foreach (var name in tagNames)
            {
                var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Name == name)
                          ?? new Tag { Name = name };
                post.PostTags.Add(new PostTag { Tag = tag });
            }

            if (mentionUserIds != null)
            {
                foreach (var uid in mentionUserIds.Distinct())
                    post.Mentions.Add(new PostMention { MentionedUserId = uid });
            }

            _db.Posts.Add(post);
            await _db.SaveChangesAsync();

            if (mentionUserIds != null && mentionUserIds.Count > 0)
            {
                var author = await _db.Users.FindAsync(userId);
                var authorLogin = author?.Login ?? "пользователь";
                foreach (var uid in mentionUserIds.Distinct().Where(id => id != userId))
                {
                    await _notify.SendSystemMessageAsync(uid,
                        $"[MENTION:{post.Id}]Пользователь @{authorLogin} отметил(а) вас в публикации");
                }
            }

            return post;
        }

        public async Task DeletePostAsync(int userId, int postId)
        {
            var post = await _db.Posts.FindAsync(postId);
            if (post != null && post.UserId == userId)
            {
                _db.Posts.Remove(post);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<Post>> SearchPostsAsync(string query)
        {
            query = query.Trim();

            if (query.StartsWith('#'))
                return await SearchByTagAsync(query.TrimStart('#'));

            var lower = query.ToLower();
            return await PostsWithIncludes()
                .Where(p => p.Content.ToLower().Contains(lower))
                .OrderByDescending(p => p.CreatedAt)
                .Take(30)
                .ToListAsync();
        }

        public async Task<List<string>> SearchTagNamesAsync(string query, int take = 10)
        {
            query = query.TrimStart('#').Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(query)) return new();
            return await _db.Tags
                .Where(t => t.Name.Contains(query))
                .OrderByDescending(t => t.PostTags.Count)
                .Select(t => t.Name)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<Post>> GetRecentAsync(int take = 50) =>
            await PostsWithIncludes()
                .OrderByDescending(p => p.CreatedAt)
                .Take(take)
                .ToListAsync();

        public async Task<List<Post>> SearchByTagAsync(string tag)
        {
            tag = tag.TrimStart('#').Trim().ToLowerInvariant();
            return await PostsWithIncludes()
                .Where(p => p.PostTags.Any(pt => pt.Tag!.Name == tag))
                .OrderByDescending(p => p.CreatedAt)
                .Take(50)
                .ToListAsync();
        }
    }
}
