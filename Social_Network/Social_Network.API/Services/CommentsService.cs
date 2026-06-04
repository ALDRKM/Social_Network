using Microsoft.EntityFrameworkCore;
using Social_Network.API.Data;
using Social_Network.Core.Models;

namespace Social_Network.API.Services
{
    public class CommentsService: ICommentsService
    {
        private readonly AppDbContext _db;
        public CommentsService(AppDbContext db) => _db = db;

        public async Task<List<Comment>> GetCommentsAsync(int postId) =>
            await _db.Comments
            .Include(com => com.User).Where(com => com.PostId == postId).OrderBy(com => com.CreatedAt).ToListAsync();

        public async Task<Comment> CreateCommentAsync(int userId, int postId, string content)
        {
            var comment = new Comment()
            {
                UserId = userId,
                PostId = postId,
                Text = content
            };
            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();
            return comment;
        }

        public async Task<bool> DeleteCommentAsync(int userId, int commentId)
        {
            var comment = await _db.Comments.FindAsync(commentId);
            if (comment == null || comment.UserId != userId ) return false;

            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
