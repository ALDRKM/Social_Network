using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Social_Network.Core.Models.DTOs;
using Social_Network.Service;
using Social_Network.Constants;

namespace Social_Network.ViewModels
{
    [QueryProperty(nameof(ChatId), "chatId")]
    [QueryProperty(nameof(OtherUserId), "otherUserId")]
    [QueryProperty(nameof(OtherLogin), "otherLogin")]
    [QueryProperty(nameof(OtherAvatar), "otherAvatar")]
    [QueryProperty(nameof(OtherOnline), "otherOnline")]
    [QueryProperty(nameof(OtherLastSeenStr), "otherLastSeen")]
    [QueryProperty(nameof(IsSystemChatStr), "isSystem")]
    public partial class ChatViewModel : BaseViewModel
    {
        private readonly IMessageService _mes;
        private readonly IChatHubService _hub;
        private readonly ISubscriptionService _sub;

        private readonly List<MessageDto> _all = new();
        public ObservableCollection<MessageItemViewModel> Messages { get; } = new();

        public ChatViewModel(IMessageService mes, IChatHubService hub, ISubscriptionService sub)
        {
            _mes = mes;
            _hub = hub;
            _sub = sub;
            Title = "Чат";
        }

        [ObservableProperty] private int chatId;
        [ObservableProperty] private int otherUserId;
        [ObservableProperty] private string otherLogin = string.Empty;
        [ObservableProperty] private string? otherAvatar;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(StatusText))]
        [NotifyPropertyChangedFor(nameof(StatusColor))]
        private bool otherOnline;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(StatusText))]
        private string otherLastSeenStr = string.Empty;

        [ObservableProperty] private string messageText = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSendMessages))]
        [NotifyPropertyChangedFor(nameof(CanOpenProfile))]
        private bool isSystemChat;

        public string IsSystemChatStr
        {
            set => IsSystemChat = bool.TryParse(value, out var b) && b;
        }

        public bool CanSendMessages => !IsSystemChat;
        public bool CanOpenProfile => !IsSystemChat && OtherUserId > 0;

        public string StatusText
        {
            get
            {
                if (IsSystemChat) return "уведомления";
                if (OtherOnline) return "в сети";
                if (DateTime.TryParse(OtherLastSeenStr, null,
                        System.Globalization.DateTimeStyles.RoundtripKind, out var ls))
                    return "был(а) в сети " + FormatLastSeen(ls);
                return "не в сети";
            }
        }

        public string StatusColor => IsSystemChat ? "#7A5C3E" : OtherOnline ? "#6FBF4B" : "#7A5C3E";

        private static string FormatLastSeen(DateTime utc)
        {
            var local = utc.ToLocalTime();
            var diff = DateTime.Now - local;
            if (diff.TotalMinutes < 1) return "только что";
            if (diff.TotalHours < 1) return $"{(int)diff.TotalMinutes} мин. назад";
            if (diff.TotalHours < 24) return $"{(int)diff.TotalHours} ч. назад";
            return local.ToString("dd.MM.yyyy HH:mm", new System.Globalization.CultureInfo("ru-RU"));
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsSearchVisible))]
        private bool isSearchOpen;

        public bool IsSearchVisible => IsSearchOpen;

        [ObservableProperty] private string searchQuery = string.Empty;

        private int _myId;

        private MessageItemViewModel Wrap(MessageDto m) => new(m, DeleteMessageAsync, IsSystemChat,
            ApproveRequestAsync, RejectRequestAsync, OpenMentionPostAsync);

        private void OnMessageReceived(MessageDto mes)
        {
            mes.IsMine = mes.SenderId == _myId;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (_all.Any(m => m.Id == mes.Id)) return;
                _all.Add(mes);
                if (MatchesFilter(mes)) Messages.Add(Wrap(mes));
            });
        }

        private void OnMessageDeleted(int mesId)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var msg = _all.FirstOrDefault(m => m.Id == mesId);
                if (msg != null) _all.Remove(msg);
                var shown = Messages.FirstOrDefault(m => m.Id == mesId);
                if (shown != null) Messages.Remove(shown);
            });
        }

        private async Task InitializeAsync(int id)
        {
            try
            {
                IsBusy = true;
                _myId = Preferences.Default.Get(AppSettings.UserIdKey, 0);

                var messages = await _mes.GetByChatIdAsync(id);
                _all.Clear();
                foreach (var m in messages)
                {
                    m.IsMine = m.SenderId == _myId;
                    _all.Add(m);
                }
                RebuildDisplay();

                foreach (var m in messages.Where(m => m.SenderId != _myId && !m.IsRead))
                    await _mes.MarksAsReadAsync(m.Id, _myId, id);

                try
                {
                    _hub.MessageReceived -= OnMessageReceived;
                    _hub.MessageDeleted -= OnMessageDeleted;
                    _hub.MessageReceived += OnMessageReceived;
                    _hub.MessageDeleted += OnMessageDeleted;
                    await _hub.StartAsync();
                    await _hub.JoinChatAsync(id, _myId);
                }
                catch { }
            }
            catch { }
            finally { IsBusy = false; }
        }

        partial void OnChatIdChanged(int value)
            => MainThread.BeginInvokeOnMainThread(async () => await InitializeAsync(value));

        partial void OnSearchQueryChanged(string value) => RebuildDisplay();

        private bool MatchesFilter(MessageDto m)
            => string.IsNullOrWhiteSpace(SearchQuery)
               || m.Text.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase);

        private void RebuildDisplay()
        {
            Messages.Clear();
            foreach (var m in _all.Where(MatchesFilter))
                Messages.Add(Wrap(m));
        }

        private async Task ApproveRequestAsync(int requestId)
        {
            await _sub.ApproveRequestAsync(requestId, _myId);
            await InitializeAsync(ChatId);
        }

        private async Task RejectRequestAsync(int requestId)
        {
            await _sub.RejectRequestAsync(requestId, _myId);
            await InitializeAsync(ChatId);
        }

        private async Task OpenMentionPostAsync(int postId)
        {
            await Shell.Current.GoToAsync($"PostDetailPage?postId={postId}");
        }

        [RelayCommand]
        private async Task Send()
        {
            if (IsSystemChat) return;
            var text = MessageText;
            if (string.IsNullOrWhiteSpace(text)) return;
            MessageText = string.Empty;
            await SendTextAsync(text);
        }

        [RelayCommand]
        private async Task Attach()
        {
            if (IsSystemChat) return;
            try
            {
                var file = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Прикрепить фото",
                    FileTypes = FilePickerFileType.Images
                });
                if (file == null) return;
                await SendTextAsync(file.FullPath);
            }
            catch { }
        }

        private async Task SendTextAsync(string text)
        {
            try
            {
                var sent = await _mes.SendAsync(ChatId, _myId, text);
                if (sent != null)
                {
                    sent.IsMine = true;
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (_all.All(m => m.Id != sent.Id))
                        {
                            _all.Add(sent);
                            if (MatchesFilter(sent)) Messages.Add(Wrap(sent));
                        }
                    });
                }
            }
            catch { }
        }

        [RelayCommand]
        private void ToggleSearch()
        {
            IsSearchOpen = !IsSearchOpen;
            if (!IsSearchOpen)
            {
                SearchQuery = string.Empty;
                RebuildDisplay();
            }
        }

        private async Task DeleteMessageAsync(MessageDto message)
        {
            if (message == null || !message.IsMine || IsSystemChat) return;
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                bool confirm = await Shell.Current.DisplayAlertAsync(
                    "Удалить сообщение?", "Сообщение будет отменено.", "Удалить", "Отмена");
                if (!confirm) return;
                if (await _mes.RevokeAsync(message.Id, ChatId))
                    OnMessageDeleted(message.Id);
            });
        }

        [RelayCommand]
        private async Task OpenOtherProfile()
        {
            if (!CanOpenProfile) return;
            await Shell.Current.GoToAsync($"OtherProfilePage?userId={OtherUserId}");
        }

        [RelayCommand]
        private async Task GoBack() => await Shell.Current.GoToAsync("//ChatListPage");

        public async Task CleanupAsync()
        {
            _hub.MessageDeleted -= OnMessageDeleted;
            _hub.MessageReceived -= OnMessageReceived;
            await _hub.LeaveChatAsync(ChatId, _myId);
            await _hub.StopAsync();
        }
    }
}
