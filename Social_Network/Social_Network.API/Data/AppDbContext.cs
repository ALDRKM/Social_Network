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
        }
    }
}
