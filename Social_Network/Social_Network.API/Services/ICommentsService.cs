using Social_Network.Core.Models;

namespace Social_Network.API.Services
{
    public interface ICommentsService
    {
        Task<List<Comment>> GetCommentsAsync(int postId);
        Task<Comment> CreateCommentAsync(int userId, int postId, string content);
        Task<bool> DeleteCommentAsync(int userId, int commentId);
    }
}
