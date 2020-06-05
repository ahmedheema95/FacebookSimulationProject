using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Project_Facebook_Application.Models
{
    public class SearchUserState
    {
        public string UserID { get; set; }
        public string FullName { get; set; }
        public string Country { get; set; }
        public byte[] ProfilePicture { get; set; }
        public string State { get; set; }
    }
}
