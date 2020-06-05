using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Project_Facebook_Application.Core.Domain
{
    public class MyUser:IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public byte[] ProfilePicture { get; set; }
        public bool Blocked { get; set; }
        public string Country { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Friend> Friends { get; set; }
        public virtual ICollection<FriendRequest> FriendRequests { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public MyUser()
        {

        }
        public MyUser(string n):base(n)
        {

        }
    }
}
