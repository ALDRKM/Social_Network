using System.Text.Json.Serialization;

namespace Social_Network.Core.Models
{
    public class Tag
    {
        public int Id { get; set; }
        // Имя тега без '#', хранится в нижнем регистре
        public string Name { get; set; } = string.Empty;

        [JsonIgnore] public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
    }
}
