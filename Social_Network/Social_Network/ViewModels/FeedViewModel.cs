using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Social_Network.Constants;
using Social_Network.Service;
using System.Collections.ObjectModel;
using Social_Network.Core.Models;

namespace Social_Network.ViewModels
{
    public partial class FeedViewModel : BaseViewModel
    {
        private readonly IPostService _post;
        private readonly ILikeService _like;

        ObservableCollection<Post> Posts { get; } = new();

        public FeedViewModel(IPostService post, ILikeService like)
        {
            _post = post;
            _like = like;
            Title = "Лента";
        }

        [RelayCommand]
        private async Task LoadFeed()
        {
            if (IsBusy) return;
            IsBusy = true;

            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            var posts = await _post.GetFeedAsync(userId);

            Posts.Clear();
            foreach (var p in posts)
            {
                Posts.Add(p);
            }

            IsBusy = false;
        }

        [RelayCommand]
        private async Task ToggleLike(Post post)
        {
            int userId = Preferences.Default.Get(AppSettings.UserIdKey,0);
            bool isLike = await _like.IsLikeAsync(userId,post.Id);

            if (!isLike)
                await _like.LikeAsync(userId, post.Id);
            else
                await _like.UnlikeAsync(userId, post.Id);

            await LoadFeed();
        }

        [RelayCommand]
        private async Task GoToPostDetail(Post post)
        {
            await Shell.Current.GoToAsync($"PostDetailPage?postId={post.Id}");
        }
        [RelayCommand]
        private async Task GoToCreatePost()
        {
            await Shell.Current.GoToAsync("CreatePostPage");
        }
    }
}
