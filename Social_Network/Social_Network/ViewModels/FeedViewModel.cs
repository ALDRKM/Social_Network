using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Social_Network.Constants;
using Social_Network.Helpers;
using Social_Network.Service;
using System.Collections.ObjectModel;
using Social_Network.Core.Models;

namespace Social_Network.ViewModels
{
    public partial class StoryItem : ObservableObject
    {
        public int UserId { get; set; }
        public int PostId { get; set; }
        public string Login { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }

        [ObservableProperty] private bool isViewed;
    }

    public enum FeedMode
    {
        All,
        Following
    }

    public enum FeedSortBy
    {
        Date,
        Popularity
    }

    public enum FeedSortOrder
    {
        Desc,
        Asc
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

        private readonly HashSet<int> _viewedStories = new();

        [ObservableProperty]
        private bool hasStories;

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsAllFeed))]
        [NotifyPropertyChangedFor(nameof(IsFollowingFeed))]
        [NotifyPropertyChangedFor(nameof(AllFeedTabColor))]
        [NotifyPropertyChangedFor(nameof(FollowingFeedTabColor))]
        private FeedMode feedMode = FeedMode.All;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DateSortTabColor))]
        [NotifyPropertyChangedFor(nameof(PopularitySortTabColor))]
        private FeedSortBy sortBy = FeedSortBy.Date;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DescSortTabColor))]
        [NotifyPropertyChangedFor(nameof(AscSortTabColor))]
        private FeedSortOrder sortOrder = FeedSortOrder.Desc;

        [ObservableProperty]
        private bool isFilterModalOpen;

        public bool IsAllFeed => FeedMode == FeedMode.All;
        public bool IsFollowingFeed => FeedMode == FeedMode.Following;
        public string AllFeedTabColor => IsAllFeed ? "#5C3210" : "#665C3210";
        public string FollowingFeedTabColor => IsFollowingFeed ? "#5C3210" : "#665C3210";

        public string DateSortTabColor => SortBy == FeedSortBy.Date ? "#5C3210" : "#665C3210";
        public string PopularitySortTabColor => SortBy == FeedSortBy.Popularity ? "#5C3210" : "#665C3210";
        public string DescSortTabColor => SortOrder == FeedSortOrder.Desc ? "#5C3210" : "#665C3210";
        public string AscSortTabColor => SortOrder == FeedSortOrder.Asc ? "#5C3210" : "#665C3210";

        public FeedViewModel(IPostService post, ILikeService like, ISavedPostService saved,
            ISubscriptionService sub, IChatService chat)
        {
            _post = post;
            _like = like;
            _saved = saved;
            _sub = sub;
            _chat = chat;
            Title = "Лента";
        }

        private static string ViewedStoriesKey(int userId) => $"viewedStories_{userId}";

        private void LoadViewedStories(int userId)
        {
            _viewedStories.Clear();
            var saved = Preferences.Default.Get(ViewedStoriesKey(userId), string.Empty);
            foreach (var part in saved.Split(',', StringSplitOptions.RemoveEmptyEntries))
                if (int.TryParse(part, out var id))
                    _viewedStories.Add(id);
        }

        private void SaveViewedStories(int userId)
        {
            Preferences.Default.Set(ViewedStoriesKey(userId), string.Join(',', _viewedStories));
        }

        private async Task RefreshUnreadAsync(int userId)
        {
            try
            {
                var chats = await _chat.GetUserChatsAsync(userId);
                AppState.HasUnreadChats = chats.Any(c => c.HasUnread);
            }
            catch { }
        }

        [RelayCommand]
        private async Task SelectAllFeed()
        {
            if (FeedMode == FeedMode.All) return;
            FeedMode = FeedMode.All;
            await LoadFeed();
        }

        [RelayCommand]
        private async Task SelectFollowingFeed()
        {
            if (FeedMode == FeedMode.Following) return;
            FeedMode = FeedMode.Following;
            await LoadFeed();
        }

        [RelayCommand]
        private void OpenFilterModal() => IsFilterModalOpen = true;

        [RelayCommand]
        private void CloseFilterModal() => IsFilterModalOpen = false;

        [RelayCommand]
        private void SelectDateSort()
        {
            if (SortBy == FeedSortBy.Date) return;
            SortBy = FeedSortBy.Date;
            ApplySort();
        }

        [RelayCommand]
        private void SelectPopularitySort()
        {
            if (SortBy == FeedSortBy.Popularity) return;
            SortBy = FeedSortBy.Popularity;
            ApplySort();
        }

        [RelayCommand]
        private void SelectDescSort()
        {
            if (SortOrder == FeedSortOrder.Desc) return;
            SortOrder = FeedSortOrder.Desc;
            ApplySort();
        }

        [RelayCommand]
        private void SelectAscSort()
        {
            if (SortOrder == FeedSortOrder.Asc) return;
            SortOrder = FeedSortOrder.Asc;
            ApplySort();
        }

        private void ApplySort()
        {
            if (Posts.Count == 0) return;

            IEnumerable<FeedPostViewModel> sorted;
            if (SortBy == FeedSortBy.Popularity)
            {
                sorted = SortOrder == FeedSortOrder.Desc
                    ? Posts.OrderByDescending(p => p.LikeCount).ThenByDescending(p => p.CreatedAt)
                    : Posts.OrderBy(p => p.LikeCount).ThenByDescending(p => p.CreatedAt);
            }
            else
            {
                sorted = SortOrder == FeedSortOrder.Desc
                    ? Posts.OrderByDescending(p => p.CreatedAt)
                    : Posts.OrderBy(p => p.CreatedAt);
            }

            var list = sorted.ToList();
            Posts.Clear();
            foreach (var p in list)
                Posts.Add(p);
        }

        [RelayCommand]
        private async Task LoadFeed()
        {
            if (IsBusy) { IsRefreshing = false; return; }
            IsBusy = true;
            try
            {
                int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
                LoadViewedStories(userId);

                var mode = FeedMode == FeedMode.Following ? "following" : "all";

                var posts = await _post.GetFeedAsync(userId, mode);
                var followingIds = (await _sub.GetFollowingIdsAsync(userId)).ToHashSet();
                var pendingIds = (await _sub.GetPendingFollowingIdsAsync(userId)).ToHashSet();
                var savedIds = (await _saved.GetSavedIdsAsync(userId)).ToHashSet();

                Posts.Clear();
                foreach (var p in posts)
                {
                    bool isLiked = p.Likes?.Any(l => l.UserId == userId) ?? false;
                    bool isSaved = savedIds.Contains(p.Id);
                    bool isFollowing = followingIds.Contains(p.UserId);
                    bool isPending = pendingIds.Contains(p.UserId);
                    Posts.Add(new FeedPostViewModel(p, isLiked, isSaved, isFollowing, isPending, userId));
                }

                ApplySort();
                BuildStories(posts, followingIds, userId);
                await RefreshUnreadAsync(userId);
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        private void SaveScrollPosition(FeedPostViewModel? item)
        {
            if (item != null)
                AppState.FeedScrollIndex = Posts.IndexOf(item);
            else
                AppState.FeedScrollIndex = 0;
            AppState.SkipFeedReload = true;
        }

        [RelayCommand]
        private async Task OpenUser(User user)
        {
            if (user == null) return;
            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            if (user.Id == userId) await Shell.Current.GoToAsync("//ProfilePage");
            else await Shell.Current.GoToAsync($"OtherProfilePage?userId={user.Id}");
        }

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
            story.IsViewed = true;
            _viewedStories.Add(story.PostId);

            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            SaveViewedStories(userId);

            int idx = Stories.IndexOf(story);
            if (idx >= 0 && idx < Stories.Count - 1)
                Stories.Move(idx, Stories.Count - 1);

            var postItem = Posts.FirstOrDefault(p => p.Id == story.PostId);
            SaveScrollPosition(postItem);
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

            if (SortBy == FeedSortBy.Popularity)
                ApplySort();
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

            if (item.IsFollowingAuthor)
            {
                foreach (var p in Posts.Where(p => p.UserId == item.UserId))
                {
                    p.IsFollowingAuthor = false;
                    p.IsPendingRequest = false;
                }
                await _sub.UnfollowAsync(userId, item.UserId);
                return;
            }

            if (item.IsPendingRequest)
            {
                foreach (var p in Posts.Where(p => p.UserId == item.UserId))
                    p.IsPendingRequest = false;
                await _sub.UnfollowAsync(userId, item.UserId);
                return;
            }

            var result = await _sub.FollowAsync(userId, item.UserId);
            bool followed = result.Status == "followed";
            bool requested = result.Status == "requested";

            foreach (var p in Posts.Where(p => p.UserId == item.UserId))
            {
                p.IsFollowingAuthor = followed;
                p.IsPendingRequest = requested;
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
            SaveScrollPosition(item);
            await Shell.Current.GoToAsync($"PostDetailPage?postId={item.Id}");
        }

        [RelayCommand]
        private async Task GoToCreatePost()
        {
            await Shell.Current.GoToAsync("CreatePostPage");
        }
    }
}
