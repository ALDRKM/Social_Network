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

        [ObservableProperty] private int postId;
        [ObservableProperty] private Post? post;
        [ObservableProperty] private bool isLiked;
        [ObservableProperty] private int likeCount;
        [ObservableProperty] private string newComment = string.Empty;

        // Если отвечаем на комментарий — id родителя и подпись
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsReplying))]
        [NotifyPropertyChangedFor(nameof(ReplyHint))]
        private int? replyToId;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ReplyHint))]
        private string replyToLogin = string.Empty;

        public bool IsReplying => ReplyToId.HasValue;
        public string ReplyHint => IsReplying ? $"Ответ @{ReplyToLogin}" : string.Empty;

        public ObservableCollection<CommentItemViewModel> Comments { get; } = new();

        private async Task LoadPost(int id)
        {
            IsBusy = true;
            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);

            Post = await _post.GetByIdAsync(id);
            IsLiked = await _like.IsLikeAsync(userId, id);
            LikeCount = await _like.GetLikeCountAsync(id);

            await ReloadCommentsAsync();

            IsBusy = false;
        }

        partial void OnPostIdChanged(int value) => MainThread.BeginInvokeOnMainThread(async () => await LoadPost(value));

        [RelayCommand]
        public async Task ToggleLike()
        {
            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);

            if (IsLiked)
            {
                await _like.UnlikeAsync(userId, PostId);
                IsLiked = false;
                LikeCount = Math.Max(0, LikeCount - 1);
            }
            else
            {
                await _like.LikeAsync(userId, PostId);
                IsLiked = true;
                LikeCount += 1;
            }
        }

        [RelayCommand]
        private void Reply(CommentItemViewModel item)
        {
            if (item == null) return;
            ReplyToId = item.Id;
            ReplyToLogin = item.AuthorLogin;
        }

        [RelayCommand]
        private void CancelReply()
        {
            ReplyToId = null;
            ReplyToLogin = string.Empty;
        }

        [RelayCommand]
        public async Task AddComment()
        {
            if (string.IsNullOrWhiteSpace(NewComment)) return;
            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);

            var comment = await _com.CreateCommentAsync(userId, PostId, NewComment, ReplyToId);
            if (comment != null)
            {
                NewComment = string.Empty;
                CancelReply();
                await ReloadCommentsAsync();
            }
        }

        [RelayCommand]
        public async Task EditComment(CommentItemViewModel item)
        {
            if (item == null || !item.IsMine) return;
            string? edited = await Shell.Current.DisplayPromptAsync(
                "Редактировать комментарий", null, "Сохранить", "Отмена", initialValue: item.Text);
            if (string.IsNullOrWhiteSpace(edited)) return;

            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            if (await _com.EditCommentAsync(userId, item.Id, edited))
                await ReloadCommentsAsync();
        }

        [RelayCommand]
        public async Task DeleteComment(CommentItemViewModel item)
        {
            if (item == null || !item.IsMine) return;
            bool confirm = await Shell.Current.DisplayAlertAsync(
                "Удалить комментарий?", "Это действие нельзя отменить.", "Удалить", "Отмена");
            if (!confirm) return;

            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            if (await _com.DeleteCommentAsync(userId, item.Id))
                await ReloadCommentsAsync();
        }

        private async Task ReloadCommentsAsync()
        {
            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            var flat = await _com.GetCommentsAsync(PostId);

            // Строим дерево: верхний уровень + ответы
            var map = flat.ToDictionary(c => c.Id, c => new CommentItemViewModel(c, userId));
            Comments.Clear();
            foreach (var c in flat)
            {
                var item = map[c.Id];
                if (c.ParentCommentId.HasValue && map.TryGetValue(c.ParentCommentId.Value, out var parent))
                    parent.Replies.Add(item);
                else
                    Comments.Add(item);
            }
        }
    }
}
