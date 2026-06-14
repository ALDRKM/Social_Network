using Social_Network.Core.Models;

namespace Social_Network.Service
{
    public interface ICommentService
    {
        Task<List<Comment>> GetCommentsAsync(int postId);
        Task<Comment?> CreateCommentAsync(int userId, int postId, string content);
        Task<bool> DeleteCommentAsync(int userId, int commentId);
    }
}
