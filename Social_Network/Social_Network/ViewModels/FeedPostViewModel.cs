using CommunityToolkit.Mvvm.ComponentModel;
using Social_Network.Core.Models;

namespace Social_Network.ViewModels
{
    public partial class FeedPostViewModel : ObservableObject
    {
        public Post Post { get; }

        public FeedPostViewModel(Post post, bool isLiked, bool isSaved, bool isFollowingAuthor,
            bool isPendingRequest, int currentUserId)
        {
            Post = post;
            this.isLiked = isLiked;
            this.isSaved = isSaved;
            this.isFollowingAuthor = isFollowingAuthor;
            this.isPendingRequest = isPendingRequest;
            likeCount = post.Likes?.Count ?? 0;
            IsOwnPost = post.UserId == currentUserId;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(LikeIcon))]
        private bool isLiked;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SaveIcon))]
        private bool isSaved;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FollowText))]
        [NotifyPropertyChangedFor(nameof(FollowColor))]
        [NotifyPropertyChangedFor(nameof(FollowTextColor))]
        private bool isFollowingAuthor;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FollowText))]
        [NotifyPropertyChangedFor(nameof(FollowColor))]
        [NotifyPropertyChangedFor(nameof(FollowTextColor))]
        private bool isPendingRequest;

        [ObservableProperty] private int likeCount;

        public bool IsOwnPost { get; }

        public string LikeIcon => IsLiked ? "icon_like_already.png" : "icon_like.png";
        public string SaveIcon => IsSaved ? "icon_saved_already.png" : "icon_button_to_save_to_favorite.png";

        public string FollowText
        {
            get
            {
                if (IsPendingRequest) return "Запрос отправлен";
                return IsFollowingAuthor ? "Отписаться" : "Подписаться";
            }
        }

        public string FollowColor => IsPendingRequest ? "#A89880" : IsFollowingAuthor ? "#C4A882" : "#C8702A";
        public string FollowTextColor => IsPendingRequest ? "#3B2A1A" : IsFollowingAuthor ? "#3B2A1A" : "White";

        public int Id => Post.Id;
        public int UserId => Post.UserId;
        public string AuthorLogin => Post.User?.Login ?? string.Empty;
        public string? AuthorAvatar => Post.User?.AvatarUrl;
        public string Content => Post.Content;
        public string? ImageUrl => Post.ImageUrl;
        public bool HasImage => !string.IsNullOrEmpty(Post.ImageUrl);
        public int CommentCount => Post.Comments?.Count ?? 0;
        public bool HasContent => !string.IsNullOrWhiteSpace(Post.Content);
        public bool IsNote => Post.Type == PostType.Note;
        public bool HasPhotoDescription => HasImage && HasContent;

        public List<string> Tags => Post.PostTags?
            .Select(pt => pt.Tag?.Name)
            .Where(n => !string.IsNullOrEmpty(n))
            .Select(n => "#" + n)
            .ToList() ?? new();
        public bool HasTags => Tags.Count > 0;

        public List<User> MentionUsers => Post.Mentions?
            .Select(m => m.MentionedUser)
            .Where(u => u != null)
            .ToList()! ?? new();
        public bool HasMentions => MentionUsers.Count > 0;
    }
}
