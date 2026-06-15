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


        [ObservableProperty]
        private int otherUserId;
        [ObservableProperty]
        private User? user;
        [ObservableProperty]
        private bool isFollowing;

        public ObservableCollection<Post> Posts { get; } = new();

        public OtherProfileViewModel(IUserService user, IPostService post, ISubscriptionService sub, IChatService chat)
        {
            _user = user;
            _sub = sub;
            _post = post; 
            _chat = chat;
        }

        private async Task LoadProfile(int targetUserId)
        {
            IsBusy = true;
            int myId = Preferences.Default.Get(AppSettings.UserIdKey,0);

            User = await _user.GetUserByIdAsync(targetUserId);
            IsFollowing = await _sub.IsFollowingAsync(myId, targetUserId);

            var posts = await _post.GetUserPostsAsync(targetUserId);

            Posts.Clear();
            foreach (var p in posts)
                Posts.Add(p);

            IsBusy = false;
        }

        partial void OnOtherUserIdChanged(int value) => MainThread.BeginInvokeOnMainThread(async () => await LoadProfile(value));


        [RelayCommand]
        public async Task ToggleFollow()
        {
            int myId = Preferences.Default.Get(AppSettings.UserIdKey,0);

            if(IsFollowing)
            {
                await _sub.UnfollowAsync(myId,OtherUserId);
                IsFollowing = false;
            }
            else
            {
                await _sub.FollowAsync(myId,OtherUserId);
                IsFollowing = true;
            }
        }

        [RelayCommand]
        public async Task OpenChat()
        {
            int myId = Preferences.Default.Get(AppSettings.UserIdKey,0);

            var chat = await _chat.CreateOrGetChatAsync(myId, OtherUserId);
            if (chat != null)
                await Shell.Current.GoToAsync($"ChatPage?chatId={chat.Id}");
        }
    }
}
