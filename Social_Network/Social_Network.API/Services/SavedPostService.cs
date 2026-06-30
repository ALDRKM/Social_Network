using Microsoft.EntityFrameworkCore;
using Social_Network.API.Data;
using Social_Network.Core.Models;

namespace Social_Network.API.Services
{
    public class SavedPostService : ISavedPostService
    {
        private readonly AppDbContext _db;
        public SavedPostService(AppDbContext db) => _db = db;

        public async Task SaveAsync(int userId, int postId)
        {
            bool already = await _db.SavedPosts.AnyAsync(s => s.UserId == userId && s.PostId == postId);
            if (already) return;

            _db.SavedPosts.Add(new SavedPost { UserId = userId, PostId = postId });
            await _db.SaveChangesAsync();
        }

        public async Task UnsaveAsync(int userId, int postId)
        {
            var saved = await _db.SavedPosts.FirstOrDefaultAsync(s => s.UserId == userId && s.PostId == postId);
            if (saved == null) return;

            _db.SavedPosts.Remove(saved);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> IsSavedAsync(int userId, int postId) =>
            await _db.SavedPosts.AnyAsync(s => s.UserId == userId && s.PostId == postId);

        public async Task<List<int>> GetSavedIdsAsync(int userId) =>
            await _db.SavedPosts
                .Where(s => s.UserId == userId)
                .Select(s => s.PostId)
                .ToListAsync();

        public async Task<List<Post>> GetSavedAsync(int userId) =>
            await _db.Posts
                .Where(p => p.SavedPosts.Any(s => s.UserId == userId))
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .OrderByDescending(p => p.SavedPosts.Where(s => s.UserId == userId).Max(s => s.SavedAt))
                .ToListAsync();
    }
}
