using System.Text.Json.Serialization;

namespace Social_Network.Core.Models
{
    // Отметка пользователя в публикации
    public class PostMention
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        [JsonIgnore] public Post? Post { get; set; }
        public int MentionedUserId { get; set; }
        public User? MentionedUser { get; set; }
    }
}
