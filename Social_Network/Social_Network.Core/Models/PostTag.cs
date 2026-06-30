using System.Text.Json.Serialization;

namespace Social_Network.Core.Models
{
    // Связь многие-ко-многим между публикациями и тегами
    public class PostTag
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        [JsonIgnore] public Post? Post { get; set; }
        public int TagId { get; set; }
        public Tag? Tag { get; set; }
    }
}
