using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Project_Facebook_Application.Core.Domain
{
    public class Like
    {
        public int LikeID { get; set; }
        public int PostID { get; set; }
        public DateTime LikeDate { get; set; }
        public string User_ID { get; set; }
        public virtual Post Post { get; set; }
        public virtual MyUser User { get; set; }
    }
}
