using Social_Network.Core.Models;

namespace Social_Network.Service
{
    public interface ICommentService
    {
        Task<List<Comment>> GetCommentsAsync(int postId);
        Task<Comment?> CreateCommentAsync(int userId, int postId, string content, int? parentCommentId = null);
        Task<bool> EditCommentAsync(int userId, int commentId, string content);
        Task<bool> DeleteCommentAsync(int userId, int commentId);
        Task<List<Comment>> GetUserCommentsAsync(int userId);
    }
}
