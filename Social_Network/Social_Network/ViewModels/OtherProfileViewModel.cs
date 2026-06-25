using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Social_Network.Core.Models;
using Social_Network.Service;
using Social_Network.Constants;

namespace Social_Network.ViewModels
{
    [QueryProperty(nameof(OtherUserId),"userId")]
    public partial class OtherProfileViewModel: BaseViewModel
    {
        private readonly IUserService _user;
        private readonly IPostService _post;
        private readonly ISubscriptionService _sub;
        private readonly IChatService _chat;
        private readonly IUserSettingsService _settings;

        [ObservableProperty] private int otherUserId;
        [ObservableProperty] private User? user;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FollowButtonText))]
        [NotifyPropertyChangedFor(nameof(FollowButtonColor))]
        [NotifyPropertyChangedFor(nameof(CanViewContent))]
        private bool isFollowing;

        public string FollowButtonColor => IsFollowing ? "#C4A882" : "#C8702A";

        [ObservableProperty] private int followersCount;
        [ObservableProperty] private int followingCount;
        [ObservableProperty] private int postsCount;

        // Приватный аккаунт
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanViewContent))]
        private bool isPrivate;

        // Вкладки Заметки / Фото
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotesTab))]
        [NotifyPropertyChangedFor(nameof(PhotosTabColor))]
        [NotifyPropertyChangedFor(nameof(NotesTabColor))]
        private bool isPhotosTab = true;

        public bool IsNotesTab => !IsPhotosTab;
        public string PhotosTabColor => IsPhotosTab ? "#5C3210" : "#665C3210";
        public string NotesTabColor => IsNotesTab ? "#5C3210" : "#665C3210";

        public string FollowButtonText => IsFollowing ? "Вы подписаны" : "Подписаться";

        // Контент виден, если аккаунт не приватный или мы уже подписаны
        public bool CanViewContent => !IsPrivate || IsFollowing;

        public ObservableCollection<Post> Photos { get; } = new();
        public ObservableCollection<Post> Notes { get; } = new();

        public OtherProfileViewModel(IUserService user, IPostService post, ISubscriptionService sub,
            IChatService chat, IUserSettingsService settings)
        {
            _user = user;
            _sub = sub;
            _post = post;
            _chat = chat;
            _settings = settings;
        }

        private async Task LoadProfile(int targetUserId)
        {
            IsBusy = true;
            int myId = Preferences.Default.Get(AppSettings.UserIdKey, 0);

            User = await _user.GetUserByIdAsync(targetUserId);
            IsFollowing = await _sub.IsFollowingAsync(myId, targetUserId);
            FollowersCount = await _sub.GetFollowersCountAsync(targetUserId);
            FollowingCount = await _sub.GetFollowingCountAsync(targetUserId);

            var settings = await _settings.GetAsync(targetUserId);
            IsPrivate = settings?.IsPrivateAccount ?? false;

            await LoadPostsAsync(targetUserId);

            IsBusy = false;
        }

        private async Task LoadPostsAsync(int targetUserId)
        {
            Photos.Clear();
            Notes.Clear();
            PostsCount = 0;

            // Если приватный и мы не подписаны — контент скрыт
            if (!CanViewContent) return;

            var photos = await _post.GetUserPostsAsync(targetUserId, PostType.Photo);
            var notes = await _post.GetUserPostsAsync(targetUserId, PostType.Note);

            foreach (var p in photos) Photos.Add(p);
            foreach (var n in notes) Notes.Add(n);
            PostsCount = Photos.Count + Notes.Count;
        }

        partial void OnOtherUserIdChanged(int value)
            => MainThread.BeginInvokeOnMainThread(async () => await LoadProfile(value));

        [RelayCommand]
        private void SelectPhotosTab() => IsPhotosTab = true;

        [RelayCommand]
        private void SelectNotesTab() => IsPhotosTab = false;

        [RelayCommand]
        public async Task ToggleFollow()
        {
            int myId = Preferences.Default.Get(AppSettings.UserIdKey, 0);

            if (IsFollowing)
            {
                await _sub.UnfollowAsync(myId, OtherUserId);
                IsFollowing = false;
            }
            else
            {
                await _sub.FollowAsync(myId, OtherUserId);
                IsFollowing = true;
            }

            FollowersCount = await _sub.GetFollowersCountAsync(OtherUserId);
            // Перезагружаем контент (для приватного аккаунта он появится/исчезнет)
            await LoadPostsAsync(OtherUserId);
        }

        [RelayCommand]
        public async Task OpenChat()
        {
            int myId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            var chat = await _chat.CreateOrGetChatAsync(myId, OtherUserId);
            if (chat == null) return;

            await Shell.Current.GoToAsync(
                $"ChatPage?chatId={chat.Id}&otherUserId={OtherUserId}" +
                $"&otherLogin={Uri.EscapeDataString(User?.Login ?? string.Empty)}" +
                $"&otherAvatar={Uri.EscapeDataString(User?.AvatarUrl ?? string.Empty)}" +
                $"&otherOnline={User?.IsOnline ?? false}" +
                $"&otherLastSeen={Uri.EscapeDataString(User?.LastSeen?.ToString("o") ?? string.Empty)}");
        }

        [RelayCommand]
        private async Task OpenPost(Post post)
            => await Shell.Current.GoToAsync($"PostDetailPage?postId={post.Id}");

        [RelayCommand]
        private async Task Share()
        {
            if (User == null) return;
            await Microsoft.Maui.ApplicationModel.DataTransfer.Share.RequestAsync(new ShareTextRequest
            {
                Title = "Профиль",
                Text = $"Профиль пользователя @{User.Login} в SocialNet"
            });
        }

        [RelayCommand]
        private async Task GoBack() => await Shell.Current.GoToAsync("..");
    }
}
