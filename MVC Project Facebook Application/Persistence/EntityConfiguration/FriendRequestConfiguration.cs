using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC_Project_Facebook_Application.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Project_Facebook_Application.Persistence.EntityConfiguration
{
    public class FriendRequestConfiguration : IEntityTypeConfiguration<FriendRequest>
    {
        public void Configure(EntityTypeBuilder<FriendRequest> builder)
        {
            builder.HasKey(f => new { f.FriendRequestID, f.User_ID });
            builder.HasOne(f => f.User).WithMany(u => u.FriendRequests).HasForeignKey(f => f.User_ID);
        }
    }
}
