using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC_Project_Facebook_Application.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Project_Facebook_Application.Persistence.EntityConfiguration
{
    public class LikeConfiguration : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.HasKey(l => new { l.LikeID, l.PostID });
            builder.HasOne(l => l.Post).WithMany(p => p.Likes).HasForeignKey(l => new { l.PostID});
            builder.HasOne(l => l.User).WithMany(u => u.Likes).HasForeignKey(l => l.User_ID);
        }
    }
}
