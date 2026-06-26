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
        private readonly IChatService _chat;

        public ObservableCollection<FeedPostViewModel> Posts { get; } = new();
        public ObservableCollection<StoryItem> Stories { get; } = new();

        // Просмотренные истории (чтобы они оставались серыми после обновления ленты)
        private readonly HashSet<int> _viewedStories = new();

        [ObservableProperty]
        private bool hasStories;

        // Отдельный флаг для RefreshView (не завязан на IsBusy-гард — иначе бесконечное обновление)
        [ObservableProperty]
        private bool isRefreshing;

        public FeedViewModel(IPostService post, ILikeService like, ISavedPostService saved, ISubscriptionService sub, IChatService chat)
        {
            _post = post;
            _like = like;
            _saved = saved;
            _sub = sub;
            _chat = chat;
            Title = "Лента";
        }

        // Обновить индикатор непрочитанных сообщений в меню навигации
        private async Task RefreshUnreadAsync(int userId)
        {
            try
            {
                var chats = await _chat.GetUserChatsAsync(userId);
                Helpers.AppState.HasUnreadChats = chats.Any(c => c.HasUnread);
            }
            catch { }
        }

        [RelayCommand]
        private async Task LoadFeed()
        {
            if (IsBusy) { IsRefreshing = false; return; }
            IsBusy = true;
            try
            {
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
                await RefreshUnreadAsync(userId);
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task OpenUser(User user)
        {
            if (user == null) return;
            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            if (user.Id == userId) await Shell.Current.GoToAsync("//ProfilePage");
            else await Shell.Current.GoToAsync($"OtherProfilePage?userId={user.Id}");
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
            if (item == null || item.IsOwnPost) return;
            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            bool follow = !item.IsFollowingAuthor;

            // Мгновенно и одинаково обновляем все карточки этого автора
            foreach (var p in Posts.Where(p => p.UserId == item.UserId))
                p.IsFollowingAuthor = follow;

            // Сервер идемпотентен, поэтому быстрые клики не ломают состояние
            if (follow) await _sub.FollowAsync(userId, item.UserId);
            else await _sub.UnfollowAsync(userId, item.UserId);
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
