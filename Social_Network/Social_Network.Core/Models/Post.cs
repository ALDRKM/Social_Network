using System;
using System.Collections.Generic;
using System.Text;

namespace Social_Network.Core.Models
{
    public class Post
    {
        // Properties
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        // Главное/обложечное изображение (для обратной совместимости и превью)
        public string? ImageUrl { get; set; }
        // Тип публикации: фото-пост или заметка
        public PostType Type { get; set; } = PostType.Photo;
        public DateTime CreatedAt { get; set; }= DateTime.UtcNow;

        // Foreign key and navigation property for User
        public int UserId { get; set; }
        public User User { get; set; }

        // Navigation properties
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<SavedPost> SavedPosts { get; set; } = new List<SavedPost>();
        public ICollection<PostImage> Images { get; set; } = new List<PostImage>();
        public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
        public ICollection<PostMention> Mentions { get; set; } = new List<PostMention>();
    }
}
