using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MVC_Project_Facebook_Application.Areas.Identity.Pages.Account
{
    public class AccessDeniedModel : PageModel
    {
        public void OnGet()
        {
            string UserIDSession = HttpContext.Session.GetString("UserID");
            ViewData["UserIDSession"] = UserIDSession;
        }
    }
}

