using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Social_Network.Constants;
using Social_Network.Service;
using Social_Network.Core.Models;

namespace Social_Network.ViewModels
{
    public partial class ProfileViewModel: BaseViewModel
    {
        private readonly IUserService _user;
        private readonly IPostService _post;
        private readonly ISubscriptionService _sub;

        public ProfileViewModel(IUserService user, IPostService post, ISubscriptionService sub)
        {
            _user = user;
            _sub = sub;
            _post = post;
        }

        [ObservableProperty] private User? user;
        [ObservableProperty] private int followersCount;
        [ObservableProperty] private int followingCount;
        [ObservableProperty] private int postsCount;

        // Вкладки: Заметки / Фото
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotesTab))]
        [NotifyPropertyChangedFor(nameof(PhotosTabColor))]
        [NotifyPropertyChangedFor(nameof(NotesTabColor))]
        private bool isPhotosTab = true;

        public bool IsNotesTab => !IsPhotosTab;
        public string PhotosTabColor => IsPhotosTab ? "#C8702A" : "#B89B73";
        public string NotesTabColor => IsNotesTab ? "#C8702A" : "#B89B73";

        // Режим редактирования (удаление постов)
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EditButtonText))]
        [NotifyPropertyChangedFor(nameof(EditButtonColor))]
        private bool isEditing;

        public string EditButtonText => IsEditing ? "Отменить" : "Редактировать";
        public string EditButtonColor => IsEditing ? "#8B5A2B" : "#C8702A";

        public ObservableCollection<Post> Photos { get; } = new();
        public ObservableCollection<Post> Notes { get; } = new();

        [RelayCommand]
        public async Task LoadProfile()
        {
            if (IsBusy) return;
            IsBusy = true;

            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);

            User = await _user.GetUserByIdAsync(userId);
            FollowersCount = await _sub.GetFollowersCountAsync(userId);
            FollowingCount = await _sub.GetFollowingCountAsync(userId);

            var photos = await _post.GetUserPostsAsync(userId, PostType.Photo);
            var notes = await _post.GetUserPostsAsync(userId, PostType.Note);

            Photos.Clear();
            foreach (var p in photos) Photos.Add(p);
            Notes.Clear();
            foreach (var n in notes) Notes.Add(n);

            PostsCount = Photos.Count + Notes.Count;

            IsBusy = false;
        }

        [RelayCommand]
        private void SelectPhotosTab() => IsPhotosTab = true;

        [RelayCommand]
        private void SelectNotesTab() => IsPhotosTab = false;

        [RelayCommand]
        private void ToggleEditing() => IsEditing = !IsEditing;

        [RelayCommand]
        private async Task DeletePost(Post post)
        {
            if (post == null) return;

            bool confirm = await Shell.Current.DisplayAlertAsync(
                "Удалить публикацию?",
                "Это действие нельзя отменить.",
                "Удалить", "Отмена");
            if (!confirm) return;

            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            await _post.DeletePostAsync(userId, post.Id);

            Photos.Remove(post);
            Notes.Remove(post);
            PostsCount = Photos.Count + Notes.Count;
        }

        [RelayCommand]
        private async Task EditPost(Post post)
        {
            if (post == null) return;
            await Shell.Current.GoToAsync($"PostDetailPage?postId={post.Id}");
        }

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
        private async Task GoToMyActions() => await Shell.Current.GoToAsync("MyActionsPage");

        [RelayCommand]
        private async Task GoToSettings() => await Shell.Current.GoToAsync("SettingsPage");
    }
}
