using System.Text.Json.Serialization;

namespace Social_Network.Core.Models
{
    // Одна из нескольких фотографий публикации (карусель)
    public class PostImage
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        [JsonIgnore] public Post? Post { get; set; }
        public string Url { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}
