using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Social_Network.Constants;
using Social_Network.Service;
using Social_Network.Core.Models;
using System.Runtime.CompilerServices;

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

        [ObservableProperty]
        private User? user;
        [ObservableProperty]
        private int followersCount;
        [ObservableProperty]
        private int followingsCount;

        public ObservableCollection<Post> Posts { get; } = new();

        [RelayCommand]
        public async Task LoadProfile()
        {
            if (IsBusy) return;
            IsBusy = true;

            int userId = Preferences.Default.Get(AppSettings.UserIdKey,0);

            User = await _user.GetUserByIdAsync(userId);
            var posts = await _post.GetUserPostsAsync(userId);

            Posts.Clear();
            foreach (var p in posts)
                Posts.Add(p);

            IsBusy = false;
        }

        [RelayCommand]
        private async Task GoToSettings()
        {
            await Shell.Current.GoToAsync("SettingsPage");
        }
    }
}
