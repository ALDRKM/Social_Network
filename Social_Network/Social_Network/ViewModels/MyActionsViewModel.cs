using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Social_Network.Core.Models;
using Social_Network.Service;
using Social_Network.Constants;

namespace Social_Network.ViewModels
{
    public partial class MyActionsViewModel: BaseViewModel
    {
        private readonly ILikeService _like;
        private readonly ICommentService _com;
        private readonly ISavedPostService _saved;
        private readonly ISubscriptionService _sub;

        public ObservableCollection<FeedPostViewModel> LikedPosts { get; } = new();
        public ObservableCollection<CommentItemViewModel> MyComments { get; } = new();
        public ObservableCollection<FeedPostViewModel> SavedPosts { get; } = new();

        public MyActionsViewModel(ILikeService like, ICommentService com, ISavedPostService saved, ISubscriptionService sub)
        {
            _like = like;
            _com = com;
            _saved = saved;
            _sub = sub;
            Title = "Мои действия";
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsLikesTab))]
        [NotifyPropertyChangedFor(nameof(IsCommentsTab))]
        [NotifyPropertyChangedFor(nameof(IsSavedTab))]
        [NotifyPropertyChangedFor(nameof(LikesTabColor))]
        [NotifyPropertyChangedFor(nameof(CommentsTabColor))]
        [NotifyPropertyChangedFor(nameof(SavedTabColor))]
        private int activeTab;

        public bool IsLikesTab => ActiveTab == 0;
        public bool IsCommentsTab => ActiveTab == 1;
        public bool IsSavedTab => ActiveTab == 2;

        // Выбранная вкладка — чуть темнее, невыбранная — светлее
        public string LikesTabColor => IsLikesTab ? "#8B4E1C" : "#C8955A";
        public string CommentsTabColor => IsCommentsTab ? "#8B4E1C" : "#C8955A";
        public string SavedTabColor => IsSavedTab ? "#8B4E1C" : "#C8955A";

        [RelayCommand]
        public async Task Load()
        {
            if (IsBusy) return;
            IsBusy = true;

            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            var followingIds = (await _sub.GetFollowingIdsAsync(userId)).ToHashSet();
            var pendingIds = (await _sub.GetPendingFollowingIdsAsync(userId)).ToHashSet();
            var savedIds = (await _saved.GetSavedIdsAsync(userId)).ToHashSet();

            var liked = await _like.GetUserLikedPostsAsync(userId);
            LikedPosts.Clear();
            foreach (var p in liked)
                LikedPosts.Add(new FeedPostViewModel(p, true, savedIds.Contains(p.Id),
                    followingIds.Contains(p.UserId), pendingIds.Contains(p.UserId), userId));

            var comments = await _com.GetUserCommentsAsync(userId);
            MyComments.Clear();
            foreach (var c in comments) MyComments.Add(new CommentItemViewModel(c, userId));

            var saved = await _saved.GetSavedAsync(userId);
            SavedPosts.Clear();
            foreach (var p in saved)
                SavedPosts.Add(new FeedPostViewModel(p, p.Likes?.Any(l => l.UserId == userId) ?? false, true,
                    followingIds.Contains(p.UserId), pendingIds.Contains(p.UserId), userId));

            IsBusy = false;
        }

        [RelayCommand] private void SelectLikesTab() => ActiveTab = 0;
        [RelayCommand] private void SelectCommentsTab() => ActiveTab = 1;
        [RelayCommand] private void SelectSavedTab() => ActiveTab = 2;

        // ----- Действия с карточками постов -----
        [RelayCommand]
        private async Task ToggleLike(FeedPostViewModel item)
        {
            if (item == null) return;
            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            if (item.IsLiked) { await _like.UnlikeAsync(userId, item.Id); item.IsLiked = false; item.LikeCount = Math.Max(0, item.LikeCount - 1); }
            else { await _like.LikeAsync(userId, item.Id); item.IsLiked = true; item.LikeCount += 1; }
        }

        [RelayCommand]
        private async Task ToggleSave(FeedPostViewModel item)
        {
            if (item == null) return;
            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            if (item.IsSaved) { await _saved.UnsaveAsync(userId, item.Id); item.IsSaved = false; }
            else { await _saved.SaveAsync(userId, item.Id); item.IsSaved = true; }
        }

        [RelayCommand]
        private async Task ToggleFollow(FeedPostViewModel item)
        {
            if (item == null || item.IsOwnPost) return;
            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);

            if (item.IsFollowingAuthor)
            {
                await _sub.UnfollowAsync(userId, item.UserId);
                foreach (var p in LikedPosts.Concat(SavedPosts).Where(p => p.UserId == item.UserId))
                {
                    p.IsFollowingAuthor = false;
                    p.IsPendingRequest = false;
                }
                return;
            }

            if (item.IsPendingRequest)
            {
                await _sub.UnfollowAsync(userId, item.UserId);
                foreach (var p in LikedPosts.Concat(SavedPosts).Where(p => p.UserId == item.UserId))
                    p.IsPendingRequest = false;
                return;
            }

            var result = await _sub.FollowAsync(userId, item.UserId);
            foreach (var p in LikedPosts.Concat(SavedPosts).Where(p => p.UserId == item.UserId))
            {
                p.IsFollowingAuthor = result.Status == "followed";
                p.IsPendingRequest = result.Status == "requested";
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
            if (item.UserId == userId) await Shell.Current.GoToAsync("//ProfilePage");
            else await Shell.Current.GoToAsync($"OtherProfilePage?userId={item.UserId}");
        }

        [RelayCommand]
        private async Task OpenPost(FeedPostViewModel item)
        {
            if (item == null) return;
            await Shell.Current.GoToAsync($"PostDetailPage?postId={item.Id}");
        }

        // ----- Комментарии -----
        [RelayCommand]
        private async Task OpenComment(CommentItemViewModel item)
        {
            if (item == null) return;
            await Shell.Current.GoToAsync($"PostDetailPage?postId={item.PostId}");
        }

        [RelayCommand]
        private async Task EditComment(CommentItemViewModel item)
        {
            if (item == null) return;
            string? edited = await Shell.Current.DisplayPromptAsync(
                "Редактировать комментарий", null, "Сохранить", "Отмена", initialValue: item.Text);
            if (string.IsNullOrWhiteSpace(edited)) return;

            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            if (await _com.EditCommentAsync(userId, item.Id, edited)) await Load();
        }

        [RelayCommand]
        private async Task DeleteComment(CommentItemViewModel item)
        {
            if (item == null) return;
            bool confirm = await Shell.Current.DisplayAlertAsync(
                "Удалить комментарий?", "Это действие нельзя отменить.", "Удалить", "Отмена");
            if (!confirm) return;

            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            if (await _com.DeleteCommentAsync(userId, item.Id)) MyComments.Remove(item);
        }

        [RelayCommand]
        private async Task GoToSettings() => await Shell.Current.GoToAsync("SettingsPage");

        [RelayCommand]
        private async Task GoBack() => await Shell.Current.GoToAsync("..");
    }
}
