using System;
using System.Collections.Generic;
using System.Text;
using Social_Network.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Social_Network.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<SavedPost> SavedPosts { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<PostMention> PostMentions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscription>().
                HasOne(f => f.Following).
                WithMany(u => u.Followers).
                HasForeignKey(s => s.FollowingId).
                OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Subscription>()
                .HasOne(f => f.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(s => s.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Chat>()
                .HasOne(u1 => u1.User1)
                .WithMany()
                .HasForeignKey(u1 => u1.User1Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Chat>()
                .HasOne(u2 => u2.User2)
                .WithMany()
                .HasForeignKey(u2 => u2.User2Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Теги: имя уникально
            modelBuilder.Entity<Tag>()
                .HasIndex(t => t.Name)
                .IsUnique();

            // Связь публикация-тег
            modelBuilder.Entity<PostTag>()
                .HasOne(pt => pt.Post)
                .WithMany(p => p.PostTags)
                .HasForeignKey(pt => pt.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PostTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.PostTags)
                .HasForeignKey(pt => pt.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            // Отметки пользователей в публикации
            modelBuilder.Entity<PostMention>()
                .HasOne(pm => pm.Post)
                .WithMany(p => p.Mentions)
                .HasForeignKey(pm => pm.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PostMention>()
                .HasOne(pm => pm.MentionedUser)
                .WithMany()
                .HasForeignKey(pm => pm.MentionedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ответы на комментарии (самосвязь)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
