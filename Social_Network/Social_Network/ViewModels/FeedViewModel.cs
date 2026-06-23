using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Social_Network.Constants;
using Social_Network.Service;
using System.Collections.ObjectModel;
using Social_Network.Core.Models;

namespace Social_Network.ViewModels
{
    // Кружок "истории" сверху ленты — подписка, опубликовавшая что-то за последние 24 часа
    public partial class StoryItem : ObservableObject
    {
        public int UserId { get; set; }
        public int PostId { get; set; }
        public string Login { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }

        // Просмотрена ли история (после просмотра — серая и уходит вправо)
        [ObservableProperty] private bool isViewed;
    }

    public partial class FeedViewModel : BaseViewModel
    {
        private readonly IPostService _post;
        private readonly ILikeService _like;
        private readonly ISavedPostService _saved;
        private readonly ISubscriptionService _sub;

        public ObservableCollection<FeedPostViewModel> Posts { get; } = new();
        public ObservableCollection<StoryItem> Stories { get; } = new();

        // Просмотренные истории (чтобы они оставались серыми после обновления ленты)
        private readonly HashSet<int> _viewedStories = new();
        private bool _togglingFollow;

        [ObservableProperty]
        private bool hasStories;

        public FeedViewModel(IPostService post, ILikeService like, ISavedPostService saved, ISubscriptionService sub)
        {
            _post = post;
            _like = like;
            _saved = saved;
            _sub = sub;
            Title = "Лента";
        }

        [RelayCommand]
        private async Task LoadFeed()
        {
            if (IsBusy) return;
            IsBusy = true;

            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);

            var posts = await _post.GetFeedAsync(userId);
            var followingIds = (await _sub.GetFollowingIdsAsync(userId)).ToHashSet();
            var savedIds = (await _saved.GetSavedIdsAsync(userId)).ToHashSet();

            Posts.Clear();
            foreach (var p in posts)
            {
                bool isLiked = p.Likes?.Any(l => l.UserId == userId) ?? false;
                bool isSaved = savedIds.Contains(p.Id);
                bool isFollowing = followingIds.Contains(p.UserId);
                Posts.Add(new FeedPostViewModel(p, isLiked, isSaved, isFollowing, userId));
            }

            BuildStories(posts, followingIds, userId);

            IsBusy = false;
        }

        // Истории: подписки, опубликовавшие что-то за последние 24 часа
        private void BuildStories(List<Post> posts, HashSet<int> followingIds, int currentUserId)
        {
            Stories.Clear();
            var since = DateTime.UtcNow.AddHours(-24);

            var recent = posts
                .Where(p => p.UserId != currentUserId && followingIds.Contains(p.UserId)
                            && p.CreatedAt >= since && p.User != null)
                .GroupBy(p => p.UserId)
                .Select(g => g.OrderByDescending(p => p.CreatedAt).First());

            foreach (var p in recent.OrderBy(p => _viewedStories.Contains(p.Id)))
            {
                Stories.Add(new StoryItem
                {
                    UserId = p.UserId,
                    PostId = p.Id,
                    Login = p.User!.Login,
                    AvatarUrl = p.User.AvatarUrl,
                    IsViewed = _viewedStories.Contains(p.Id)
                });
            }

            HasStories = Stories.Count > 0;
        }

        [RelayCommand]
        private async Task OpenStory(StoryItem story)
        {
            if (story == null) return;
            // Просмотрено: становится серым, уходит в конец и запоминается
            story.IsViewed = true;
            _viewedStories.Add(story.PostId);
            int idx = Stories.IndexOf(story);
            if (idx >= 0 && idx < Stories.Count - 1)
                Stories.Move(idx, Stories.Count - 1);
            await Shell.Current.GoToAsync($"PostDetailPage?postId={story.PostId}");
        }

        [RelayCommand]
        private async Task ToggleLike(FeedPostViewModel item)
        {
            if (item == null) return;
            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);

            if (item.IsLiked)
            {
                await _like.UnlikeAsync(userId, item.Id);
                item.IsLiked = false;
                item.LikeCount = Math.Max(0, item.LikeCount - 1);
            }
            else
            {
                await _like.LikeAsync(userId, item.Id);
                item.IsLiked = true;
                item.LikeCount += 1;
            }
        }

        [RelayCommand]
        private async Task ToggleSave(FeedPostViewModel item)
        {
            if (item == null) return;
            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);

            if (item.IsSaved)
            {
                await _saved.UnsaveAsync(userId, item.Id);
                item.IsSaved = false;
            }
            else
            {
                await _saved.SaveAsync(userId, item.Id);
                item.IsSaved = true;
            }
        }

        [RelayCommand]
        private async Task ToggleFollow(FeedPostViewModel item)
        {
            if (item == null || item.IsOwnPost || _togglingFollow) return;
            _togglingFollow = true;
            try
            {
                int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
                bool follow = !item.IsFollowingAuthor;

                // Сразу обновляем все карточки этого автора (один логический клик)
                foreach (var p in Posts.Where(p => p.UserId == item.UserId))
                    p.IsFollowingAuthor = follow;

                if (follow) await _sub.FollowAsync(userId, item.UserId);
                else await _sub.UnfollowAsync(userId, item.UserId);
            }
            finally
            {
                _togglingFollow = false;
            }
        }

        [RelayCommand]
        private async Task Share(FeedPostViewModel item)
        {
            if (item == null) return;
            await Microsoft.Maui.ApplicationModel.DataTransfer.Share.RequestAsync(new ShareTextRequest
            {
                Title = "Публикация",
                Text = $"@{item.AuthorLogin}: {item.Content}"
            });
        }

        [RelayCommand]
        private async Task OpenProfile(FeedPostViewModel item)
        {
            if (item == null) return;
            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            if (item.UserId == userId)
                await Shell.Current.GoToAsync("//ProfilePage");
            else
                await Shell.Current.GoToAsync($"OtherProfilePage?userId={item.UserId}");
        }

        [RelayCommand]
        private async Task GoToPostDetail(FeedPostViewModel item)
        {
            if (item == null) return;
            await Shell.Current.GoToAsync($"PostDetailPage?postId={item.Id}");
        }

        [RelayCommand]
        private async Task GoToCreatePost()
        {
            await Shell.Current.GoToAsync("CreatePostPage");
        }
    }
}
