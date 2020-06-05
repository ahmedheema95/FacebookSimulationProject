using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVC_Project_Facebook_Application.Core.Domain;
using MVC_Project_Facebook_Application.Persistence.EntityConfiguration;

namespace MVC_Project_Facebook_Application.Data
{
    public class ApplicationDbContext : IdentityDbContext<MyUser,MyRole,string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration<Post>(new PostConfiguration());
            builder.ApplyConfiguration<Friend>(new FriendConfiguration());
            builder.ApplyConfiguration<FriendRequest>(new FriendRequestConfiguration());
            builder.ApplyConfiguration<Comment>(new CommentConfiguration());
            builder.ApplyConfiguration<Like>(new LikeConfiguration());
            base.OnModelCreating(builder);
        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
    }
}
