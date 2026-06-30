using System.Collections.ObjectModel;
using Social_Network.Core.Models;

namespace Social_Network.ViewModels
{
    // Обёртка над комментарием: владелец (для кнопок ред./удал.) + ветка ответов
    public class CommentItemViewModel
    {
        public Comment Comment { get; }
        public bool IsMine { get; }
        public ObservableCollection<CommentItemViewModel> Replies { get; } = new();

        public CommentItemViewModel(Comment comment, int currentUserId)
        {
            Comment = comment;
            IsMine = comment.UserId == currentUserId;
        }

        public int Id => Comment.Id;
        public int PostId => Comment.PostId;
        public string Text => Comment.Text;
        public string AuthorLogin => Comment.User?.Login ?? string.Empty;
        public string? AuthorAvatar => Comment.User?.AvatarUrl;
        public DateTime CreatedAt => Comment.CreatedAt;
        public bool HasReplies => Replies.Count > 0;
    }
}
