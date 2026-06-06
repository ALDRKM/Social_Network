using Social_Network.API.Services;
using Microsoft.AspNetCore.Mvc;


namespace Social_Network.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController: ControllerBase
    {
        private readonly IMessageService _mes;
        public MessageController(IMessageService mes) => _mes = mes;

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
            return Ok(mes);
        }

        [HttpPut("{messageId}/read")]
        public async Task<IActionResult> MarkAsRead([FromRoute] int messageId)
        {
            await _mes.MarkIsReadAsync(messageId);
            return Ok();
        }

        [HttpDelete("{messageId}")]
        public async Task<IActionResult> RevokeMessage([FromRoute] int messageId)
        {
            bool deletemes = await _mes.RevokeMessageAsync(messageId);
            if (!deletemes) return NotFound();
            return NoContent();
        }
    }
}
