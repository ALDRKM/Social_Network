using CommunityToolkit.Mvvm.ComponentModel;
using Social_Network.Core.Models;

namespace Social_Network.ViewModels
{
    // Обёртка над публикацией с наблюдаемым состоянием (лайк/сохранение/подписка),
    // чтобы обновлять одну карточку, а не всю ленту
    public partial class FeedPostViewModel : ObservableObject
    {
        public Post Post { get; }

        public FeedPostViewModel(Post post, bool isLiked, bool isSaved, bool isFollowingAuthor, int currentUserId)
        {
            Post = post;
            this.isLiked = isLiked;
            this.isSaved = isSaved;
            this.isFollowingAuthor = isFollowingAuthor;
            likeCount = post.Likes?.Count ?? 0;
            IsOwnPost = post.UserId == currentUserId;
        }

        [ObservableProperty] private bool isLiked;
        [ObservableProperty] private bool isSaved;
        [ObservableProperty] private bool isFollowingAuthor;
        [ObservableProperty] private int likeCount;

        public bool IsOwnPost { get; }

        // Удобные геттеры для привязок
        public int Id => Post.Id;
        public int UserId => Post.UserId;
        public string AuthorLogin => Post.User?.Login ?? string.Empty;
        public string? AuthorAvatar => Post.User?.AvatarUrl;
        public string Content => Post.Content;
        public string? ImageUrl => Post.ImageUrl;
        public bool HasImage => !string.IsNullOrEmpty(Post.ImageUrl);
        public int CommentCount => Post.Comments?.Count ?? 0;
        public bool HasContent => !string.IsNullOrWhiteSpace(Post.Content);

        // Теги для красивого отображения (чипами)
        public List<string> Tags => Post.PostTags?
            .Select(pt => pt.Tag?.Name)
            .Where(n => !string.IsNullOrEmpty(n))
            .Select(n => "#" + n)
            .ToList() ?? new();
        public bool HasTags => Tags.Count > 0;
    }
}
