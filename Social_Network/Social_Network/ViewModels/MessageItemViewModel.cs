using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Social_Network.Core.Models.DTOs;
using Social_Network.Helpers;

namespace Social_Network.ViewModels
{
    public partial class MessageItemViewModel : ObservableObject
    {
        public MessageDto Dto { get; }
        private readonly Func<MessageDto, Task> _delete;
        private readonly Func<int, Task>? _approveRequest;
        private readonly Func<int, Task>? _rejectRequest;
        private readonly Func<int, Task>? _openMentionPost;

        public MessageItemViewModel(MessageDto dto, Func<MessageDto, Task> delete,
            bool isSystemChat = false,
            Func<int, Task>? approveRequest = null,
            Func<int, Task>? rejectRequest = null,
            Func<int, Task>? openMentionPost = null)
        {
            Dto = dto;
            _delete = delete;
            IsSystemChat = isSystemChat;
            _approveRequest = approveRequest;
            _rejectRequest = rejectRequest;
            _openMentionPost = openMentionPost;
        }

        public bool IsSystemChat { get; }

        public int Id => Dto.Id;
        public bool IsMine => Dto.IsMine;
        public string Text => SystemMessageHelper.GetDisplayText(Dto.Text);
        public DateTime SentAt => Dto.SentAt;

        public bool IsSubscriptionRequest =>
            IsSystemChat && !IsMine && SystemMessageHelper.IsSubscriptionRequest(Dto.Text);

        public int? SubscriptionRequestId =>
            SystemMessageHelper.GetSubscriptionRequestId(Dto.Text);

        public bool IsMentionNotification =>
            IsSystemChat && !IsMine && Dto.Text.StartsWith("[MENTION:", StringComparison.Ordinal);

        public int? MentionPostId => SystemMessageHelper.GetMentionPostId(Dto.Text);

        [RelayCommand]
        private Task Delete() => _delete(Dto);

        [RelayCommand]
        private Task ApproveRequest()
        {
            var id = SubscriptionRequestId;
            return id.HasValue && _approveRequest != null ? _approveRequest(id.Value) : Task.CompletedTask;
        }

        [RelayCommand]
        private Task RejectRequest()
        {
            var id = SubscriptionRequestId;
            return id.HasValue && _rejectRequest != null ? _rejectRequest(id.Value) : Task.CompletedTask;
        }

        [RelayCommand]
        private Task OpenMentionPost()
        {
            var id = MentionPostId;
            return id.HasValue && _openMentionPost != null ? _openMentionPost(id.Value) : Task.CompletedTask;
        }
    }
}
