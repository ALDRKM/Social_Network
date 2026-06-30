using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Social_Network.Core.Models;
using Social_Network.Service;
using Social_Network.Constants;

namespace Social_Network.ViewModels
{
    public partial class PostViewModel : BaseViewModel
    {
        private readonly IPostService _post;
        private readonly ILikeService _like;
        public PostViewModel(IPostService post, ILikeService like)
        {
            _post = post;
            _like = like;
        }

        [ObservableProperty]
        private Post post = null!;
        [ObservableProperty]
        private bool isLike;
        [ObservableProperty]
        private int likeCount;

        public async Task InitializeAsync(Post post)
        {
            Post = post;
            int userId = Preferences.Default.Get(AppSettings.UserIdKey,0);
            IsLike = await _like.IsLikeAsync(userId, post.Id);
            LikeCount = await _like.GetLikeCountAsync(post.Id);
        }

        [RelayCommand]
        private async Task ToggleLike()
        {
            int userId = Preferences.Default.Get(AppSettings.UserIdKey,0);

            if (IsLike)
            {
                await _like.UnlikeAsync(userId, Post.Id);
                IsLike= false;
                LikeCount--;
            }
            else
            {
                await _like.LikeAsync(userId, Post.Id);
                IsLike= true;
                LikeCount++;
            }
        }
    }
}
