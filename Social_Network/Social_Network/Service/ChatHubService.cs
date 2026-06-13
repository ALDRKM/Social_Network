using Microsoft.AspNetCore.SignalR.Client;
using Social_Network.Constants;
using Social_Network.Core.Models.DTOs;

namespace Social_Network.Service
{
    public class ChatHubService: IChatHubService, IAsyncDisposable
    {
        private HubConnection? _connect;

        public event Action<MessageDto>? MessageReceived;
        public event Action<int>? MessageDeleted;
        public event Action<int, int>? MessageRead;


        public async Task StartAsync()
        {
            _connect = new HubConnectionBuilder()
                .WithUrl(ApiConfig.HubUrl)
                .WithAutomaticReconnect()
                .Build();

            _connect.On<MessageDto>("MessageReceived", msg => MessageReceived?.Invoke(msg));
            _connect.On<int>("MessageDeleted", msgId => MessageDeleted?.Invoke(msgId));
            _connect.On<int, int>("MessageRead", (msgId, userId) => MessageRead?.Invoke(msgId, userId));

            await _connect.StartAsync();
        }

        public async Task StopAsync()
        {
            if(_connect != null)
            {
                await _connect.StopAsync();
            }
        }

        public async Task JoinChatAsync(int chatId, int userId)
        {
            if (_connect?.State == HubConnectionState.Connected)
                await _connect.InvokeAsync("JoinChat", chatId, userId);
        }

        public async Task LeaveChatAsync(int chatId, int userId)
        {
            if (_connect?.State == HubConnectionState.Connected)
                await _connect.InvokeAsync("LeaveChat", chatId);
        }

        public async ValueTask DisposeAsync()
        {
            if(_connect != null)
            {
                await _connect.DisposeAsync();
            }
        }
    }
}
