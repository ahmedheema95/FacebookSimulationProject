using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Project_Facebook_Application.Core.Domain
{
    public class Comment
    {
        public int CommentID { get; set; }
        public int Post_ID { get; set; }
        public string CommentText { get; set; }
        public DateTime CommentData { get; set; }
        public string User_ID { get; set; }
        public virtual MyUser User { get; set; }
        public virtual Post Post { get; set; }

    }
}
