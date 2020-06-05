using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC_Project_Facebook_Application.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Project_Facebook_Application.Persistence.EntityConfiguration
{
    public class FriendConfiguration : IEntityTypeConfiguration<Friend>
    {
        public void Configure(EntityTypeBuilder<Friend> builder)
        {
            builder.HasKey(f => new { f.FriendID, f.User_ID });
            builder.HasOne(f => f.User).WithMany(u => u.Friends).HasForeignKey(f => f.User_ID);

        }
    }
}
