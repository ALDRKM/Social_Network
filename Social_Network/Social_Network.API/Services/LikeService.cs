using Microsoft.EntityFrameworkCore;
using Social_Network.API.Data;
using Social_Network.Core.Models;

namespace Social_Network.API.Services
{
    public class LikeService : ILikeService
    {
        private readonly AppDbContext _db;
        public LikeService(AppDbContext db) => _db = db;

        public async Task CreateLikeAsync(int userId, int postId)
        {
            bool already = await _db.Likes.AnyAsync(l => l.UserId == userId && l.PostId == postId);
            if (already) return;

            var like = new Like()
            {
                UserId = userId,
                PostId = postId
            };

            _db.Likes.Add(like);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteLikeAsync(int userId, int postId)
        {
            var like = await _db.Likes.FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);
            if (like == null) return;
            _db.Likes.Remove(like);
            await _db.SaveChangesAsync();
        }

        public async Task<int> GetCountLikeAsync(int postId) => await _db.Likes.CountAsync(l => l.PostId == postId);

        public async Task<bool> IsLikeAsync(int userId, int postId) => await _db.Likes.AnyAsync(l => l.UserId == userId
        && l.PostId == postId);
    }
}
