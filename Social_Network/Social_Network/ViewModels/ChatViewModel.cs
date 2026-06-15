using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Social_Network.Core.Models.DTOs;
using Social_Network.Service;
using Social_Network.Constants;

namespace Social_Network.ViewModels
{
    [QueryProperty(nameof(ChatId),"chatId")]
    public partial class ChatViewModel: BaseViewModel
    {
        private readonly IMessageService _mes;
        private readonly IChatHubService _hub;

        public ObservableCollection<MessageDto> Messages { get; } = new();

        public ChatViewModel(IMessageService mes, IChatHubService hub)
        {
            _mes = mes;
            _hub = hub;
            Title = "Чат";
        }

        [ObservableProperty]
        private int chatId;
        [ObservableProperty]
        private string messageText = string.Empty;

        private int _myId;

        private void OnMessageReceived(MessageDto mes)
        {
            MainThread.BeginInvokeOnMainThread(() => Messages.Add(mes));
        }

        private void OnMessageDeleted(int mesId)
        {
            var msg = Messages.FirstOrDefault(m => m.Id == mesId);

            if (msg != null)
                MainThread.BeginInvokeOnMainThread(()=>Messages.Remove(msg));
        }


        private async Task InitializeAsync(int id)
        {
            IsBusy = true;
            _myId = Preferences.Default.Get(AppSettings.UserIdKey,0);

            var messages = await _mes.GetByChatIdAsync(id);
            Messages.Clear();
            foreach (var m in messages)
                Messages.Add(m);

            _hub.MessageReceived += OnMessageReceived;
            _hub.MessageDeleted += OnMessageDeleted;
            _hub.StartAsync();
            _hub.JoinChatAsync(id,_myId);

            IsBusy = false;
        }

        partial void OnChatIdChanged(int value) => MainThread.BeginInvokeOnMainThread(async () => await InitializeAsync(value));


        [RelayCommand]
        private async Task Send()
        {
            if (string.IsNullOrWhiteSpace(MessageText)) return;
            await _mes.SendAsync(ChatId,_myId, MessageText);
            MessageText = string.Empty;
        }

        public async Task CleanupAsync()
        {
            _hub.MessageDeleted -= OnMessageDeleted;
            _hub.MessageReceived -= OnMessageReceived;
            _hub.LeaveChatAsync(ChatId,_myId);
            _hub.StopAsync();
        }

            

    }
}
