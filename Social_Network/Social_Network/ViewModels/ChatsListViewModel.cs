using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Social_Network.Core.Models;
using Social_Network.Core.Models.DTOs;
using Social_Network.Service;
using Social_Network.Constants;

namespace Social_Network.ViewModels
{
    public partial class ChatsListViewModel: BaseViewModel
    {
        private readonly IChatService _chat;
        private readonly IUserService _user;

        // Существующие чаты (сверху — последние переписки)
        public ObservableCollection<ChatDto> Chats { get; } = new();
        // Найденные пользователи (для начала новой переписки)
        public ObservableCollection<User> FoundUsers { get; } = new();

        public ChatsListViewModel(IChatService chat, IUserService user)
        {
            _chat = chat;
            _user = user;
            Title = "Сообщения";
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsSearching))]
        private string searchQuery = string.Empty;

        public bool IsSearching => !string.IsNullOrWhiteSpace(SearchQuery);

        [RelayCommand]
        public async Task LoadChats()
        {
            if (IsBusy) return;
            IsBusy = true;

            int myId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            var chats = await _chat.GetUserChatsAsync(myId);

            Chats.Clear();
            // Последние переписки сверху
            foreach (var c in chats.OrderByDescending(c => c.LastMessage?.SentAt ?? c.CreatedAt))
                Chats.Add(c);

            IsBusy = false;
        }

        partial void OnSearchQueryChanged(string value) => _ = SearchUsersAsync();

        private async Task SearchUsersAsync()
        {
            var q = SearchQuery?.Trim().TrimStart('@') ?? string.Empty;
            if (string.IsNullOrEmpty(q))
            {
                FoundUsers.Clear();
                return;
            }

            int myId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            var users = await _user.SearchUsersAsync(q);
            FoundUsers.Clear();
            foreach (var u in users.Where(u => u.Id != myId))
                FoundUsers.Add(u);
        }

        [RelayCommand]
        private async Task OpenChat(ChatDto chat)
        {
            if (chat == null) return;
            await Shell.Current.GoToAsync(
                $"ChatPage?chatId={chat.Id}&otherUserId={chat.OtherUserId}" +
                $"&otherLogin={Uri.EscapeDataString(chat.OtherUserLogin)}" +
                $"&otherAvatar={Uri.EscapeDataString(chat.OtherUserAvatarUrl ?? string.Empty)}" +
                $"&otherOnline={chat.OtherUserIsOnline}");
        }

        [RelayCommand]
        private async Task StartChat(User user)
        {
            if (user == null) return;
            int myId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            var chat = await _chat.CreateOrGetChatAsync(myId, user.Id);
            if (chat == null) return;

            SearchQuery = string.Empty;
            FoundUsers.Clear();
            await Shell.Current.GoToAsync(
                $"ChatPage?chatId={chat.Id}&otherUserId={user.Id}" +
                $"&otherLogin={Uri.EscapeDataString(user.Login)}" +
                $"&otherAvatar={Uri.EscapeDataString(user.AvatarUrl ?? string.Empty)}" +
                $"&otherOnline={user.IsOnline}");
        }
    }
}
