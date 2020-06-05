using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC_Project_Facebook_Application.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Project_Facebook_Application.Persistence.EntityConfiguration
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(p => new { p.PostID });
            builder.Property(p => p.PostText).HasColumnType("text");
            builder.HasOne(p => p.User).WithMany(u => u.Posts).HasForeignKey(p => p.User_ID);
        }
    }
}
