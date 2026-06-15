using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Social_Network.Core.Models.DTOs;
using Social_Network.Service;
using Social_Network.Constants;

namespace Social_Network.ViewModels
{
    public partial class ChatsListViewModel: BaseViewModel
    {
        private readonly IChatService _chat;
        public ObservableCollection<ChatDto> Chats { get; } = new();

        public ChatsListViewModel(IChatService chat)
        {
            _chat = chat;
            Title = "Список чатов";
        }

        [RelayCommand]
        public async Task LoadChats()
        {
            if (IsBusy) return;
            IsBusy = true;

            int myId = Preferences.Default.Get(AppSettings.UserIdKey,0);
            var chats = await _chat.GetUserChatsAsync(myId);

            Chats.Clear();
            foreach (var c in chats)
                Chats.Add(c);

            IsBusy = false;

        }

        [RelayCommand]
        private async Task OpenChat(ChatDto chat)
        {
            await Shell.Current.GoToAsync($"ChatPage?chatId={chat.Id}");
        }
        
    }
}
