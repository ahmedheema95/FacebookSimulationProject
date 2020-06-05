using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_Project_Facebook_Application.Core.Domain;
using MVC_Project_Facebook_Application.Data;
using MVC_Project_Facebook_Application.Models;

namespace MVC_Project_Facebook_Application.Controllers
{
    
    public class AdminController : Controller
    {
        private readonly RoleManager<MyRole> _roleManager;
        private readonly UserManager<MyUser> _userManager;
        private ApplicationDbContext DB;
        public AdminController(ApplicationDbContext _DB, UserManager<MyUser> userManager, RoleManager<MyRole> roleManager)
        {
            DB = _DB;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        [Authorize(Roles ="Admin")]
        public IActionResult AdminSearch()
        {
            string UserIDSession = HttpContext.Session.GetString("UserID");

            /*If Session Ends Redirect To Login Page*/
            if (UserIDSession == null)
                return LocalRedirect("/Identity/Account/Login");
            /*******************************************************/

            ViewBag.UserIDSession = UserIDSession;
            ViewData["UserIDSession"] = UserIDSession;
            ViewBag.UserRole = new SelectList(DB.Roles, "Name", "Name");
            return View();
        }



        #region Admin_Operations

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> CreateUser(RegisterViewModel Input)
        {
            var user = new MyUser { UserName = Input.Email, Email = Input.Email, FirstName = Input.FirstName, LastName = Input.LastName, Age = Input.Age, Country = Input.Country };
            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                //if (!await _roleManager.RoleExistsAsync(Input.UserRole))
                //{
                //    await _roleManager.CreateAsync(new MyRole() { Name = Input.UserRole, Description = Input.Description });
                //}
                await _userManager.AddToRoleAsync(user, Input.UserRole);
                return Json("Success");
            }
            return Json(null);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public string BlockUser(string BlockedUserID)
        {
            var BlockedUser = DB.Users.Find(BlockedUserID);
            BlockedUser.Blocked = true;
            DB.SaveChanges();
            return "Blocked";
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public string UnBlockUser(string BlockedUserID)
        {
            var BlockedUser = DB.Users.Find(BlockedUserID);
            BlockedUser.Blocked = false;
            DB.SaveChanges();
            return "UnBlocked";
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateRole()
        {
            return PartialView();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> CreateRole(MyRole NewRole)
        {
            if (!await _roleManager.RoleExistsAsync(NewRole.Name))
            {
                await _roleManager.CreateAsync(new MyRole(NewRole.Name, NewRole.Description));
                return Json("RoleCreated");
            }
            return Json("RoleExists");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AssignRule(string UserID)
        {
            string RoleID = DB.UserRoles.Where(u => u.UserId == UserID).Select(r => r.RoleId).FirstOrDefault();
            string CurrentRoleName = DB.Roles.Where(r => r.Id == RoleID).Select(r => r.Name).FirstOrDefault();
            ViewBag.UserRoleModify = new SelectList(DB.Roles, "Name", "Name", CurrentRoleName);
            ViewBag.UserID = UserID;
            return PartialView();
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> AssignRuleAsync(string UserID, string RoleName)
        {
            var UpdatedRoleId = DB.UserRoles.SingleOrDefault(ur => ur.UserId == UserID);
            var userToUpdateRole = DB.Users.Where(s => s.Id == UserID).FirstOrDefault();
            if(UpdatedRoleId != null)
                DB.UserRoles.Remove(UpdatedRoleId);
            await _userManager.AddToRoleAsync(userToUpdateRole, RoleName);
            DB.SaveChanges();
            return Json("RuleChanged");
        }
        #endregion
    }
}