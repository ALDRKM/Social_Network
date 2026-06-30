using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Social_Network.API.Hubs;
using Social_Network.API.Services;
using Social_Network.Core.Models.DTOs;


namespace Social_Network.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController: ControllerBase
    {
        private readonly IMessageService _mes;
        private readonly IHubContext<ChatHub, IChatClient> _hub;
        public MessageController(IMessageService mes, IHubContext<ChatHub, IChatClient> hub)
        {
            _mes = mes;
            _hub = hub;
        }


        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetMessages([FromRoute] int chatId)
        {
            var messages = await _mes.GetByChatIdAsync(chatId);
            return Ok(messages);
        }

        public record SendMessageRequest(int ChatId, int SenderId, string Text);
        [HttpPost]
        public async Task<IActionResult> Send([FromBody] SendMessageRequest req)
        {
            var mes = await _mes.SendAsync(req.ChatId, req.SenderId, req.Text);

            await _hub.Clients.Group($"chat_{req.ChatId}").MessageReceivedAsync(mes);

            return Ok(mes);
        }

        [HttpPut("{messageId}/read")]
        public async Task<IActionResult> MarkAsRead([FromRoute] int messageId, [FromQuery] int userId, [FromQuery] int chatId)
        {
            await _mes.MarkIsReadAsync(messageId);

            await _hub.Clients.Group($"chat_{chatId}").MessageReadAsync(messageId, userId);

            return Ok();
        }

        [HttpDelete("{messageId}")]
        public async Task<IActionResult> RevokeMessage([FromRoute] int messageId, [FromQuery] int chatId)
        {
            bool deletemes = await _mes.RevokeMessageAsync(messageId);
            if (!deletemes) return NotFound();

            await _hub.Clients.Group($"chat_{chatId}").MessageDeletedAsync(messageId);

            return NoContent();
        }
    }
}
