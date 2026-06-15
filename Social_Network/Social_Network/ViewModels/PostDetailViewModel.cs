using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Social_Network.Core.Models;
using Social_Network.Service;
using Social_Network.Constants;


namespace Social_Network.ViewModels
{
    [QueryProperty(nameof(PostId),"postId")]
    public partial class PostDetailViewModel: BaseViewModel
    {
        private readonly IPostService _post;
        private readonly ICommentService _com;
        private readonly ILikeService _like;

        public PostDetailViewModel(IPostService post, ICommentService com, ILikeService like)
        {
            _post = post;
            _like = like;
            _com = com;
        }

        [ObservableProperty]
        private int postId;
        [ObservableProperty]
        private Post? post;
        [ObservableProperty]
        private bool isLiked;
        [ObservableProperty]
        private int likeCount;
        [ObservableProperty]
        private string newComment = string.Empty;

        public ObservableCollection<Comment?> Comments { get; } = new();

        private async Task LoadPost(int id)
        {
            IsBusy = true;
            int userId = Preferences.Default.Get(AppSettings.UserIdKey,0);

            Post = await _post.GetByIdAsync(id);
            IsLiked = await _like.IsLikeAsync(userId, id);
            LikeCount = await _like.GetLikeCountAsync(id);

            var comments = await _com.GetCommentsAsync(id);
            Comments.Clear();
            foreach (var com in comments)
                Comments.Add(com);

            IsBusy = false;
        }

        partial void OnPostIdChanged(int value) => MainThread.BeginInvokeOnMainThread(async () => await LoadPost(value));

        [RelayCommand]
        public async Task ToggleLike()
        {
            int userId = Preferences.Default.Get(AppSettings.UserIdKey,0);
            
            if(IsLiked)
            {
                await _like.UnlikeAsync(userId, PostId);
                IsLiked = false;
                LikeCount--;
            }
            else
            {
                await _like.LikeAsync(userId, PostId);
                IsLiked = true;
                LikeCount++;
            }
        }

        [RelayCommand]
        public async Task AddComment()
        {
            if (string.IsNullOrWhiteSpace(NewComment)) return;
            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            var comment = await _com.CreateCommentAsync(userId, PostId, NewComment);
            if (comment != null)
                Comments.Add(comment);
            NewComment = string.Empty;
        }

        [RelayCommand]
        public async Task DeleteComment(Comment comment)
        {
            int userId = Preferences.Default.Get(AppSettings.UserIdKey,0);
            bool deleted = await _com.DeleteCommentAsync(userId,comment.Id);
            if (deleted) Comments.Remove(comment);
        }
    }
}
