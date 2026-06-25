using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Social_Network.Core.Models.DTOs;
using Social_Network.Service;
using Social_Network.Constants;

namespace Social_Network.ViewModels
{
    [QueryProperty(nameof(ChatId),"chatId")]
    [QueryProperty(nameof(OtherUserId),"otherUserId")]
    [QueryProperty(nameof(OtherLogin),"otherLogin")]
    [QueryProperty(nameof(OtherAvatar),"otherAvatar")]
    [QueryProperty(nameof(OtherOnline),"otherOnline")]
    [QueryProperty(nameof(OtherLastSeenStr),"otherLastSeen")]
    public partial class ChatViewModel: BaseViewModel
    {
        private readonly IMessageService _mes;
        private readonly IChatHubService _hub;

        private readonly List<MessageDto> _all = new();
        public ObservableCollection<MessageItemViewModel> Messages { get; } = new();

        public ChatViewModel(IMessageService mes, IChatHubService hub)
        {
            _mes = mes;
            _hub = hub;
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

        public string StatusText
        {
            get
            {
                if (OtherOnline) return "в сети";
                if (DateTime.TryParse(OtherLastSeenStr, null,
                        System.Globalization.DateTimeStyles.RoundtripKind, out var ls))
                    return "был(а) в сети " + FormatLastSeen(ls);
                return "не в сети";
            }
        }
        public string StatusColor => OtherOnline ? "#6FBF4B" : "#7A5C3E";

        private static string FormatLastSeen(DateTime utc)
        {
            var local = utc.ToLocalTime();
            var diff = DateTime.Now - local;
            if (diff.TotalMinutes < 1) return "только что";
            if (diff.TotalHours < 1) return $"{(int)diff.TotalMinutes} мин. назад";
            if (diff.TotalHours < 24) return $"{(int)diff.TotalHours} ч. назад";
            return local.ToString("dd.MM.yyyy HH:mm", new System.Globalization.CultureInfo("ru-RU"));
        }

        // Поиск по сообщениям в чате
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsSearchVisible))]
        private bool isSearchOpen;

        public bool IsSearchVisible => IsSearchOpen;

        [ObservableProperty] private string searchQuery = string.Empty;

        private int _myId;

        private MessageItemViewModel Wrap(MessageDto m) => new(m, DeleteMessageAsync);

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

                // Отмечаем входящие как прочитанные (убирает точку «новое»)
                foreach (var m in messages.Where(m => m.SenderId != _myId && !m.IsRead))
                    await _mes.MarksAsReadAsync(m.Id, _myId, id);

                // Реалтайм через SignalR — не должен ронять страницу при сбое подключения
                try
                {
                    _hub.MessageReceived -= OnMessageReceived;
                    _hub.MessageDeleted -= OnMessageDeleted;
                    _hub.MessageReceived += OnMessageReceived;
                    _hub.MessageDeleted += OnMessageDeleted;
                    await _hub.StartAsync();
                    await _hub.JoinChatAsync(id, _myId);
                }
                catch { /* без реалтайма сообщения всё равно отправляются по HTTP */ }
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

        [RelayCommand]
        private async Task Send()
        {
            var text = MessageText;
            if (string.IsNullOrWhiteSpace(text)) return;
            MessageText = string.Empty;
            await SendTextAsync(text);
        }

        // Скрепка — прикрепить фото (отправляем путь, в чате отрисуется картинкой)
        [RelayCommand]
        private async Task Attach()
        {
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
                    // Показываем сразу (если реалтайм не доставит — дедуп по Id)
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

        // Удаление (отмена отправки) своего сообщения
        private async Task DeleteMessageAsync(MessageDto message)
        {
            if (message == null || !message.IsMine) return;
            bool confirm = await Shell.Current.DisplayAlertAsync(
                "Удалить сообщение?", "Сообщение будет отменено.", "Удалить", "Отмена");
            if (!confirm) return;

            if (await _mes.RevokeAsync(message.Id, ChatId))
                OnMessageDeleted(message.Id);
        }

        // По нажатию на аватар собеседника — переход в его профиль
        [RelayCommand]
        private async Task OpenOtherProfile()
        {
            if (OtherUserId <= 0) return;
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
