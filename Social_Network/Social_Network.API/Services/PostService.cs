using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Social_Network.API.Data;
using Social_Network.Core.Models;

namespace Social_Network.API.Services
{
    public class PostService: IPostService
    {
        private readonly AppDbContext _db;
        public PostService(AppDbContext db) => _db = db;

        private static readonly Regex HashtagRegex = new(@"#(\w+)", RegexOptions.Compiled);

        // Полная выборка публикации со всеми связями
        private IQueryable<Post> PostsWithIncludes() =>
            _db.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .Include(p => p.Images)
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.Mentions).ThenInclude(m => m.MentionedUser);

        public async Task<Post?> GetByIdAsync(int postId) =>
            await PostsWithIncludes().FirstOrDefaultAsync(p => p.Id == postId);

        public async Task<List<Post>> GetFeedAsync(int userId)
        {
            // Лента показывает все публикации приложения
            var posts = await PostsWithIncludes()
                .OrderByDescending(post => post.CreatedAt)
                .ToListAsync();

            // Сортировка по тегам, которые лайкал пользователь:
            // посты с "любимыми" тегами поднимаются выше (при этом сохраняется свежесть)
            var likedTagIds = await _db.Likes
                .Where(l => l.UserId == userId)
                .SelectMany(l => l.Post!.PostTags.Select(pt => pt.TagId))
                .ToListAsync();

            if (likedTagIds.Count > 0)
            {
                var liked = likedTagIds.ToHashSet();
                posts = posts
                    .OrderByDescending(p => p.PostTags.Any(pt => liked.Contains(pt.TagId)))
                    .ThenByDescending(p => p.CreatedAt)
                    .ToList();
            }

            return posts;
        }

        public async Task<List<Post>> GetUserPostsAsync(int userId, PostType? type = null)
        {
            var query = PostsWithIncludes().Where(post => post.UserId == userId);
            if (type.HasValue)
                query = query.Where(post => post.Type == type.Value);

            return await query.OrderByDescending(post => post.CreatedAt).ToListAsync();
        }

        public async Task<Post> CreatePostAsync(int userId, string content, PostType type,
            List<string>? imageUrls = null, List<string>? tags = null, List<int>? mentionUserIds = null)
        {
            var post = new Post
            {
                UserId = userId,
                Content = content ?? string.Empty,
                Type = type,
                ImageUrl = imageUrls?.FirstOrDefault()
            };

            // Фотографии (карусель)
            if (imageUrls != null)
            {
                int order = 0;
                foreach (var url in imageUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
                    post.Images.Add(new PostImage { Url = url, Order = order++ });
            }

            // Теги: из текста (#тег) + переданные явно
            var tagNames = HashtagRegex.Matches(content ?? string.Empty)
                .Select(m => m.Groups[1].Value)
                .Concat(tags ?? Enumerable.Empty<string>())
                .Select(t => t.TrimStart('#').Trim().ToLowerInvariant())
                .Where(t => t.Length > 0)
                .Distinct()
                .ToList();

            foreach (var name in tagNames)
            {
                var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Name == name)
                          ?? new Tag { Name = name };
                post.PostTags.Add(new PostTag { Tag = tag });
            }

            // Отметки пользователей
            if (mentionUserIds != null)
            {
                foreach (var uid in mentionUserIds.Distinct())
                    post.Mentions.Add(new PostMention { MentionedUserId = uid });
            }

            _db.Posts.Add(post);
            await _db.SaveChangesAsync();
            return post;
        }

        public async Task DeletePostAsync(int userId, int postId)
        {
            var post = await _db.Posts.FindAsync(postId);
            if(post != null && post.UserId == userId)
            {
                _db.Posts.Remove(post);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<Post>> SearchPostsAsync(string query)
        {
            query = query.Trim();

            // Поиск по тегу, если строка начинается с '#'
            if (query.StartsWith('#'))
                return await SearchByTagAsync(query.TrimStart('#'));

            var lower = query.ToLower();
            return await PostsWithIncludes()
                .Where(p => p.Content.ToLower().Contains(lower))
                .OrderByDescending(p => p.CreatedAt)
                .Take(30)
                .ToListAsync();
        }

        // Подсказки существующих тегов (для динамического выбора при создании)
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

        // Последние публикации в приложении (для экрана поиска)
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
