using Social_Network.API.Services;
using Microsoft.AspNetCore.Mvc;


namespace Social_Network.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController: ControllerBase
    {
        private readonly IChatService _chat;
        public ChatController(IChatService chat) => _chat = chat;

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserChats([FromRoute]int userId)
        {
            var chats = await _chat.GetUserChatsAsync(userId);
            return Ok(chats);
        }

        public record CreateChatRequest(int User1Id, int User2Id);

        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] CreateChatRequest chat)
        {
            var newchat = await _chat.CreateOrGetChatUserAsync(chat.User1Id, chat.User2Id);
            return Ok(newchat);
        }

        [HttpDelete("{chatId}")]
        public async Task<IActionResult> DeleteChat([FromRoute]int chatId)
        {
            bool delchat = await _chat.DeleteChatAsync(chatId);
            if (!delchat) return NotFound();
            return NoContent();
        }
    }
}
