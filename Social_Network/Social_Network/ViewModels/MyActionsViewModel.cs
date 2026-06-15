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
        private readonly IPostService _post;

        public ObservableCollection<Post> Posts { get; } = new();

        public MyActionsViewModel(IPostService post)
        {
            _post = post;
            Title = "Мои действия";
        }

        [RelayCommand]
        public async Task LoadMyPosts()
        {
            if (IsBusy) return;
            IsBusy = true;

            int userId = Preferences.Default.Get(AppSettings.UserIdKey,0);
            var posts = await _post.GetUserPostsAsync(userId);

            Posts.Clear();
            foreach (var p in posts)
                Posts.Add(p);

            IsBusy = false;

        }

        [RelayCommand]
        private async Task DeletePost(Post post)
        {
            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            await _post.DeletePostAsync(userId,post.Id);
            Posts.Remove(post);
        }

        [RelayCommand]
        private async Task GoToCreatePost()
        {
            await Shell.Current.GoToAsync("CreatePostPage");
        }
    }
}
