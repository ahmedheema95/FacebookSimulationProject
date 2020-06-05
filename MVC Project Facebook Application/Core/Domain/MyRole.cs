using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Project_Facebook_Application.Core.Domain
{
    public class MyRole : IdentityRole
    {
        public string Description { get; set; }
        public MyRole()
        {

        }
        public MyRole(string _rolename) : base(_rolename)
        {

        }

        public MyRole(string _rolename, string _roledescription) : base(_rolename)
        {
            Description = _roledescription;
        }
    }
}
