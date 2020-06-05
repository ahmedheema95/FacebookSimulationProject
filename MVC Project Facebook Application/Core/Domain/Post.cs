using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Project_Facebook_Application.Core.Domain
{
    public class Post
    {
        public int PostID { get; set; }
        public string User_ID { get; set; }
        public string PostText { get; set; }
        public DateTime PostDate { get; set; }
        public bool IsDeleted { get; set; }
        public virtual MyUser User { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
