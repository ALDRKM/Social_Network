using System;
using System.Collections.Generic;
using System.Text;

namespace Social_Network.Core.Models.DTOs
{
    public class ChatDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int OtherUserId { get; set; }
        public string OtherUserLogin { get; set; } = string.Empty;
        public string? OtherUserAvatarUrl { get; set; } 
        public MessageDto? LastMessage { get; set; }
    }
}
