using Microsoft.EntityFrameworkCore;
using Social_Network.API.Data;
using Social_Network.Core.Models;

namespace Social_Network.API.Services
{
    public class PostService: IPostService
    {
        private readonly AppDbContext _db;
        public PostService(AppDbContext db) => _db = db;
        public async Task<List<Post>> GetFeedAsync(int userId)
        {

            var followingIds = await _db.Subscriptions
                .Where(follow => follow.FollowerId == userId)
                .Select(follow => follow.FollowingId)
                .ToListAsync();

            return await _db.Posts
                .Include(post => post.User)
                .Include(post => post.Comments)
                .Include(post => post.Likes)
                .Where(post => followingIds.Contains(post.UserId) || post.UserId == userId)
                .OrderByDescending(post => post.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Post>> GetUserPostsAsync(int userId)
        {
            return await _db.Posts
                .Include(post => post.User)
                .Include(post => post.Comments)
                .Include(post => post.Likes)
                .Where(post => post.UserId == userId)
                .OrderByDescending(post => post.CreatedAt)
                .ToListAsync();
        }

        public async Task<Post> CreatePostAsync(int userId, string content, string? imageURL)
        {
            var post = new Post()
            {
                UserId = userId,
                Content = content,
                ImageUrl = imageURL
            };
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
    }
}
