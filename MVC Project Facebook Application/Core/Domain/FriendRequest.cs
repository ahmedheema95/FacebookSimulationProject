using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Project_Facebook_Application.Core.Domain
{
    public class FriendRequest
    {
        public string FriendRequestID { get; set; }
        public string User_ID { get; set; }
        public virtual MyUser User { get; set; }
    }
}
