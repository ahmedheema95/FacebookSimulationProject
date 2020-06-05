using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC_Project_Facebook_Application.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Project_Facebook_Application.Persistence.EntityConfiguration
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(c => new { c.CommentID, c.Post_ID });
            
            builder.HasOne(c => c.Post).WithMany(p => p.Comments).HasForeignKey(c => new { c.Post_ID });
            builder.HasOne(c => c.User).WithMany(u => u.Comments).HasForeignKey(c => c.User_ID);
        }
    }
}
