using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Social_Network.Core.Models
{
    public class Comment
    {
        // Properties
        public int Id {  get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Foreign keys and navigation properties for Post and User
        public int PostId { get; set; }
        [JsonIgnore] public Post Post { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        // Ответ на другой комментарий (ветка обсуждения)
        public int? ParentCommentId { get; set; }
        [JsonIgnore] public Comment? ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }
}
