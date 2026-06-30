using Microsoft.AspNetCore.SignalR;
using Social_Network.API.Services;

namespace Social_Network.API.Hubs
{
    public class ChatHub: Hub<IChatClient>
    {
        private readonly IChatService _chat;

        public ChatHub(IChatService chat) => _chat = chat;

        private static string GroupName(int chatId) => $"chat_{chatId}";

        public async Task JoinChat(int chatId, int userId)
        {
            bool hasAccess = await _chat.UserHasAccessToChatAsync(userId, chatId);

            if (!hasAccess)
            {
                throw new HubException($"Ползователь {userId} не имеет доступа к чату {chatId}");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(chatId));
        }

        public async Task LeaveChat(int chatId) => await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(chatId));

        public override async Task OnDisconnectedAsync(Exception? ex) => await base.OnDisconnectedAsync(ex);
    }
}
