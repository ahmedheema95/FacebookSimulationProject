using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using MVC_Project_Facebook_Application.Core.Domain;
using MVC_Project_Facebook_Application.Data;
using MVC_Project_Facebook_Application.Models;

namespace MVC_Project_Facebook_Application.Controllers
{
    public class UserController : Controller
    {
        private ApplicationDbContext DB;
        public UserController(ApplicationDbContext _DB)
        {
            DB = _DB;
        }
        [Authorize]
        public IActionResult Index()
        {
            string UserIDSession = HttpContext.Session.GetString("UserID");

            /*If Session Ends Redirect To Login Page*/
            if(UserIDSession == null)
                return LocalRedirect("/Identity/Account/Login");
            /*******************************************************/

            ViewBag.UserIDSession = UserIDSession;
            List<string> FriendsID = new List<string>();
            List<Post> Posts = new List<Post>();

            FriendsID = DB.Friends.Where(f => f.User_ID == HttpContext.Session.GetString("UserID")).Select(f => f.FriendID).ToList();
            FriendsID.Add(HttpContext.Session.GetString("UserID"));

            for (int i = 0; i < FriendsID.Count(); i++)
            {
                Posts.AddRange(DB.Posts.Where(p => p.User_ID == FriendsID[i] && p.IsDeleted == false).Include(u => u.User).Include(l => l.Likes
                ).Select(p => p).ToList());
            }

            Posts = Posts.OrderByDescending(p => p.PostDate).ToList();
            ViewBag.UserID = UserIDSession;
            ViewBag.User = DB.Users.SingleOrDefault(u => u.Id == UserIDSession);
            return View(Posts);
        }



        [HttpGet]
        [Authorize]
        public IActionResult Profile(string ProfileID)
        {
            string UserIDSession = HttpContext.Session.GetString("UserID");

            /*If Session Ends Redirect To Login Page*/
            if (UserIDSession == null)
                return LocalRedirect("/Identity/Account/Login");
            /*******************************************************/

            ViewBag.UserIDSession = UserIDSession;
            ViewData["UserIDSession"] = UserIDSession;
            #region profile Image
            //ViewBag.ProfilePicture = DB.Users.SingleOrDefault(f => f.Id == ProfileID).ProfilePicture;
            #endregion
            if (ProfileID == UserIDSession || ProfileID == null)
            {
                //Check whether to display profile of logged in user or another profile
                var posts = DB.Posts.Where(p => p.User_ID == UserIDSession && p.IsDeleted == false).OrderByDescending(p => p.PostDate)
                 .Include(c => c.Comments).Include(l => l.Likes)
                 .Include(u => u.User).ToList();
                ViewBag.User = DB.Users.SingleOrDefault(u => u.Id == UserIDSession);
                var f = DB.Friends.Where(f => f.User_ID == UserIDSession).Select(p => p.FriendID);
                //my addition is ==> && u.Id != UserIDSession
                ViewBag.Friends = DB.Users.Where(u => f.Contains(u.Id) && u.Id != UserIDSession).Select(u => u).Take(6).ToList();
                // ViewBag.Friends = f;
                var fr = DB.FriendRequests.Where(f => f.User_ID == UserIDSession).Select(p => p.FriendRequestID);
                ViewBag.FriendRequest = DB.Users.Where(u => fr.Contains(u.Id)).Select(u => u).Take(6).ToList();


                return View(posts);
            }
            else
            {
                var posts = DB.Posts.Where(p => p.User_ID == ProfileID && p.IsDeleted == false).OrderByDescending(p => p.PostDate)
                             .Include(c => c.Comments).Include(l => l.Likes)
                             .Include(u => u.User).ToList();
                ViewBag.User = DB.Users.SingleOrDefault(u => u.Id == ProfileID);
                var f = DB.Friends.Where(f => f.User_ID == ProfileID).Select(p => p.FriendID);
                ViewBag.Friends = DB.Users.Where(u => f.Contains(u.Id)).Select(u => u).ToList();


                ViewBag.ProfileUser = DB.Users.Where(s => s.Id == ProfileID).FirstOrDefault();
                if (DB.Friends.FirstOrDefault(s => s.User_ID == UserIDSession && s.FriendID == ProfileID) != null)
                    ViewBag.FriendShipState = "isFriend";
                else if (DB.Friends.FirstOrDefault(s => s.User_ID == UserIDSession && s.FriendID == ProfileID) == null 
                    && DB.FriendRequests.FirstOrDefault(s => s.User_ID == UserIDSession && s.FriendRequestID == ProfileID) == null
                    && DB.FriendRequests.FirstOrDefault(s => s.User_ID == ProfileID && s.FriendRequestID == UserIDSession) == null)
                    ViewBag.FriendShipState = "notFriend";
                else if (DB.FriendRequests.FirstOrDefault(s => s.User_ID == UserIDSession && s.FriendRequestID == ProfileID) != null)
                    ViewBag.FriendShipState = "receivedFriendRequest";
                else if (DB.FriendRequests.FirstOrDefault(s => s.User_ID == ProfileID && s.FriendRequestID == UserIDSession) != null)
                    ViewBag.FriendShipState = "sentFriendRequest";

                    return View(posts);
            }

        }
        [HttpPost]
        public IActionResult EditPhoto(IFormFile img)
        {
            string ProfileID = HttpContext.Session.GetString("UserID");
            var Result = DB.Users.SingleOrDefault(u => u.Id == ProfileID);
            if (img!=null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    img.CopyToAsync(ms);
                    Result.ProfilePicture = ms.ToArray();
                }
            }
            DB.SaveChanges();
            return RedirectToAction("Profile",
                new RouteValueDictionary(new { controller = "User", action = "Main", ProfileID = ProfileID }));
        }

        public IActionResult profilename()
        {
            string UserIDSession = HttpContext.Session.GetString("UserID");
            var u = DB.Users.Where(c => c.Id == UserIDSession).FirstOrDefault();

            ViewBag.username = u.FirstName;
            return View();
        }
        #region SearchMethods
        //[HttpPost]
        //[Authorize]
        //public IActionResult SearchResult()
        //{

        //    var UserData = DB.Users.
        //        Select(s => new { FullName = s.FirstName + " " + s.LastName, s.Id, s.UserName, s.Blocked }).ToList();
        //    return Json(UserData);
        //}

        /*SearchResult() returns json file contain all users information to search method which works in the frontend*/
        [HttpPost]
        [Authorize]
        public IActionResult SearchResult()
        {
            List<string> UsersIDs = new List<string>();
            List<object> UsersDataJson = new List<object>();

            string RoleID = "";
            string RoleName = "";

            UsersIDs = DB.Users.Select(u => u.Id).ToList();

            for (int i = 0; i < UsersIDs.Count(); i++)
            {
                var UserName_blocked = DB.Users.Where(u => u.Id == UsersIDs[i]).Select(u => new { u.UserName, u.Blocked ,u.FirstName , u.LastName }).FirstOrDefault();
                RoleID = DB.UserRoles.Where(u => u.UserId == UsersIDs[i]).Select(r => r.RoleId).FirstOrDefault();
                RoleName = DB.Roles.Where(r => r.Id == RoleID).Select(r => r.Name).FirstOrDefault();
                UsersDataJson.Add(new { id = UsersIDs[i], userName = UserName_blocked.UserName, blocked = UserName_blocked.Blocked, roleName = RoleName, roleID = RoleID ,
                    FullName = UserName_blocked.FirstName + " " + UserName_blocked .LastName});
            }

            return Json(UsersDataJson.ToList());
        }




        [HttpGet]
        [Authorize]
        public IActionResult Search(string querySearch)
        {
            string UserIDSession = HttpContext.Session.GetString("UserID");
            List<SearchUserState> FilteredSearchResults = new List<SearchUserState>();
            FilteredSearchResults.AddRange(DB.Users.Where(s => (s.FirstName + " " + s.LastName).ToLower().StartsWith(querySearch)
            || s.UserName.ToLower().StartsWith(querySearch))/*.Except(DB.Users.Where(s => s.Id == UserIDSession))*/
                .Select(n => new SearchUserState { FullName = (n.FirstName + " " + n.LastName), Country = n.Country, UserID = n.Id, ProfilePicture = n.ProfilePicture, State = "" }).ToList());
            var UserFriendsIds = DB.Friends.Where(s => s.User_ID == UserIDSession).Select(s => s.FriendID);
            var UserFriendRequestedIds = DB.FriendRequests.Where(s => s.User_ID == UserIDSession || s.FriendRequestID == UserIDSession);
            foreach (var searchResult in FilteredSearchResults)
            {
                searchResult.State = "notFriend";
                foreach (var friendID in UserFriendsIds)
                {
                    if (searchResult.UserID == friendID)
                    {
                        searchResult.State = "isFriend";
                        break;
                    }
                }
                foreach (var friendRequestID in UserFriendRequestedIds)
                {
                    if (searchResult.UserID == friendRequestID.User_ID)
                        searchResult.State = "sentFriendRequest";
                    else if (searchResult.UserID == friendRequestID.FriendRequestID)
                        searchResult.State = "receivedFriendRequest";
                }
            }
            ViewBag.UserIDSession = UserIDSession;
            ViewData["UserIDSession"] = UserIDSession;
            //return Json(FilteredSearchResults);  
            return View(FilteredSearchResults);
        }
        #endregion

        #region Posts_CRUD_Operations
        [HttpPost]
        [Authorize]
        public IActionResult CreatePost(string newPostText)
        {
            string UserIDSession = HttpContext.Session.GetString("UserID");

            /*If Session Ends Redirect To Login Page*/
            if (UserIDSession == null)
                return LocalRedirect("/Identity/Account/Login");
            /*******************************************************/

            ViewBag.UserIDSession = UserIDSession;
            Post p = new Post();
            p.PostText = newPostText;
            p.PostDate = DateTime.Now;
            p.IsDeleted = false;
            p.User_ID = UserIDSession;
            p.User = DB.Users.Where(s => s.Id == UserIDSession).FirstOrDefault();
            DB.Posts.Add(p);
            DB.SaveChanges();
            return PartialView(p);
        }
        [HttpGet]
        [Authorize]
        public IActionResult EditPost(int? post_id)
        {
            string UserIDSession = HttpContext.Session.GetString("UserID");

            /*If Session Ends Redirect To Login Page*/
            if (UserIDSession == null)
                return LocalRedirect("/Identity/Account/Login");
            /*******************************************************/

            Post p = DB.Posts.Where(p => p.PostID == post_id).FirstOrDefault();
            return PartialView(p);
        }
        [HttpPost]
        [Authorize]
        public JsonResult EditPost(Post p)
        {
            string UserIDSession = HttpContext.Session.GetString("UserID");

            /*If Session Ends Redirect To Login Page*/
            if (UserIDSession == null)
                return Json("Session Ended");
            /*******************************************************/

            Post ptoUpdate = DB.Posts.Where(s => s.PostID == p.PostID).FirstOrDefault();
            ptoUpdate.PostID = p.PostID;
            ptoUpdate.PostText = p.PostText;
            ptoUpdate.PostDate = p.PostDate;
            ptoUpdate.IsDeleted = false;
            ptoUpdate.User_ID = p.User_ID;
            DB.SaveChanges();
            return Json("Edited");
        }
        [HttpPost]
        [Authorize]
        public JsonResult DeletePost(int post_id)
        {
            string UserIDSession = HttpContext.Session.GetString("UserID");

            /*If Session Ends Redirect To Login Page*/
            if (UserIDSession == null)
                return Json("Session Ended");
            /*******************************************************/

            var pos = DB.Posts.Where(c => c.PostID == post_id).FirstOrDefault();
            DB.Posts.Remove(pos);
            DB.SaveChanges();
            return Json("Deleted");
            // return View("Index");
        }
        #endregion

        #region Likes_CRUD_Operations
        [Authorize]
        public IActionResult Viewalllik(int postID)
        {
            string UserIDSession = HttpContext.Session.GetString("UserID");
            /*If Session Ends Redirect To Login Page*/
            if (UserIDSession == null)
                return LocalRedirect("/Identity/Account/Login");
            /*******************************************************/

            var AllLikes = DB.Likes.Where(s => s.PostID == postID).Include(n => n.User).ToList();
            var TargetPost = DB.Posts.Where(s => s.PostID == postID).FirstOrDefault();
            //string TargetPostText = TargetPost.PostText;
            //int TargetPostID = TargetPost.PostID;
            //ViewBag.TargetPostText = TargetPostText;
            // viewBag.TargetPostID = TargetPostID;
            // return View(AllLikes);
            return PartialView(AllLikes);
        }
        [Authorize]
        public string AddLike(int post_id)
        {
            string UserIDSession = HttpContext.Session.GetString("UserID");

            /*If Session Ends Redirect To Login Page*/
            if (UserIDSession == null)
                return "Session Ended";
            /*******************************************************/

            var post = DB.Posts.Where(c => c.PostID == post_id).FirstOrDefault();
            Like l = DB.Likes.Where(c => c.PostID == post_id).FirstOrDefault();
            Like g = DB.Likes.Where(c => c.PostID == post_id && c.User_ID == UserIDSession).FirstOrDefault();

            if (g == null)
            {
                var lik = new Like();
                lik.LikeDate = DateTime.Now;
                lik.PostID = post_id;
                if (l == null) lik.LikeID = 1;
                else { lik.LikeID = DB.Likes.Where(s => s.PostID == post_id).Max(s => s.LikeID) + 1; }


                lik.User_ID = UserIDSession;
                DB.Likes.Add(lik);
                DB.SaveChanges();
                return "Liked";
            }
            else
            {
                DB.Likes.Remove(g);
                DB.SaveChanges();
                return "UnLiked";
            }
            //    return View("Index");
        }
        /* public void RemoveLike(int Like_id, int postID)
         {

             DB.Remove(DB.Likes.Where(s => s.LikeID == Like_id && s.PostID == postID).FirstOrDefault());
             DB.SaveChanges();
            // return View("Index");
         }*/
        #endregion

        #region Comments_CRUD_Operations
        [HttpGet]
        [Authorize]
        public IActionResult ViewAllComments(int postID)
        {
            string UserIDSession = HttpContext.Session.GetString("UserID");

            /*If Session Ends Redirect To Login Page*/
            if (UserIDSession == null)
                return LocalRedirect("/Identity/Account/Login");
            /*******************************************************/

            ViewBag.UserIDSession = UserIDSession;
            ViewData["UserIDSession"] = UserIDSession;
            var AllComments = DB.Comments.Where(s => s.Post_ID == postID).Include(n => n.User)
                .Include(n => n.Post).ToList();
            var TargetPost = DB.Posts.Where(s => s.PostID == postID).FirstOrDefault();
            string TargetPostText = TargetPost.PostText;
            ViewBag.TargetPostText = TargetPostText;
            return PartialView(AllComments);
        }
        [HttpGet]
        [Authorize]
        public IActionResult AddComment(int postID)
        {
            string UserIDSession = HttpContext.Session.GetString("UserID");

            /*If Session Ends Redirect To Login Page*/
            if (UserIDSession == null)
                return LocalRedirect("/Identity/Account/Login");
            /*******************************************************/

            var TargetPost = DB.Posts.Where(s => s.PostID == postID).FirstOrDefault();
            string TargetPostText = TargetPost.PostText;
            int TargetPostID = TargetPost.PostID;
            ViewBag.userIDSession = UserIDSession;
            ViewBag.TargetPostText = TargetPostText;
            ViewBag.TargetPostID = TargetPostID;
            return PartialView();
        }
        [HttpPost]
        [Authorize]
        public JsonResult AddComment(Comment newComment)
        {
            string UserIDSession = HttpContext.Session.GetString("UserID");


            /*If Session Ends Redirect To Login Page*/
            if (UserIDSession == null)
                return Json("Session Ended");
            /*******************************************************/

            if (ModelState.IsValid)
            {
                Comment l = DB.Comments.Where(c => c.Post_ID == newComment.Post_ID).FirstOrDefault();
                newComment.CommentData = DateTime.Now;
                if (l == null) 
                    newComment.CommentID = 1;
                else
                    newComment.CommentID = DB.Comments.Where(s => s.Post_ID == newComment.Post_ID).Max(s => s.CommentID) + 1;
                DB.Comments.Add(newComment);
                DB.SaveChanges();
                return Json("CommentAdded");
            }
            return Json("CommentNotAdded");
        }

        [HttpPost]
        [Authorize]
        public string DeleteComment(int postID, int commentID)
        {

            string UserIDSession = HttpContext.Session.GetString("UserID");

            /*In Case Session Ended*/
            if (UserIDSession == null)
                return "Session Ended";
            /*******************************************************/

            DB.Remove(DB.Comments.Where(s => s.CommentID == commentID && s.Post_ID == postID).FirstOrDefault());
            DB.SaveChanges();
            return "CommentRemoved";
        }
        #endregion

        #region Friends_CRUD_Operations
        [HttpPost]
        [Authorize]
        public IActionResult FriendRequests(string UserID)
        {
            string UserIDSession = HttpContext.Session.GetString("UserID");

            /*If Session Ends Redirect To Login Page*/
            if (UserIDSession == null)
                return LocalRedirect("/Identity/Account/Login");
            /*******************************************************/

            var AllFriendReQuests = DB.FriendRequests.Where(friendRequest => friendRequest.User_ID == UserID).Select(s => s.FriendRequestID).ToList();
            List<MyUser> RequestedUsers = new List<MyUser>();
            foreach (var item in AllFriendReQuests)
                RequestedUsers.Add(DB.Users.Where(s => s.Id == item).FirstOrDefault());
            ViewBag.State = UserIDSession == UserID;
            return PartialView(RequestedUsers);
        }
        [HttpPost]
        [Authorize]
        public string AcceptFriendRequest(string FriendID)
        {
            string UserIDSession = HttpContext.Session.GetString("UserID");

            /*In Case Session Ended*/
            if (UserIDSession == null)
                return "Session Ended";
            /*******************************************************/

            var NewFriend1 = new Friend() { User_ID = UserIDSession, FriendID = FriendID };
            var NewFriend2 = new Friend() { User_ID = FriendID, FriendID = UserIDSession };
            DB.Friends.Add(NewFriend1);
            DB.Friends.Add(NewFriend2);
            var FriendRequestToDelete = DB.FriendRequests.FirstOrDefault(f => f.FriendRequestID == FriendID && f.User_ID == UserIDSession);
            DB.FriendRequests.Remove(FriendRequestToDelete);
            DB.SaveChanges();
            return "RequestAccepted";
        }
        [HttpPost]
        [Authorize]
        public string DeclineFriendRequest(string FriendID)
        {
            string UserIDSession = HttpContext.Session.GetString("UserID");

            /*In Case Session Ended*/
            if (UserIDSession == null)
                return "Session Ended";
            /*******************************************************/

            var FriendRequestToDelete = DB.FriendRequests.FirstOrDefault(f => f.FriendRequestID == FriendID && f.User_ID == UserIDSession);
            DB.FriendRequests.Remove(FriendRequestToDelete);
            DB.SaveChanges();
            return "RequestDeclined";
        }
        [HttpPost]
        [Authorize]
        public string DeleteFriendRequest(string FriendID)
        {
            string UserIDSession = HttpContext.Session.GetString("UserID");

            /*In Case Session Ended*/
            if (UserIDSession == null)
                return "Session Ended";
            /*******************************************************/

            var FriendRequestToDelete = DB.FriendRequests.FirstOrDefault(f => f.FriendRequestID == UserIDSession && f.User_ID == FriendID);
            DB.FriendRequests.Remove(FriendRequestToDelete);
            DB.SaveChanges();
            return "RequestDeleted";
        }
        [HttpPost]
        [Authorize]
        public string AddFriend(string FriendID)
        {
            string UserIDSession = HttpContext.Session.GetString("UserID");

            /*In Case Session Ended*/
            if (UserIDSession == null)
                return "Session Ended";
            /*******************************************************/

            var FirendToAddID = DB.Users.FirstOrDefault(f => f.Id == FriendID).Id;
            var FriendRequest = new FriendRequest() { User_ID = FirendToAddID, FriendRequestID = UserIDSession };
            DB.FriendRequests.Add(FriendRequest);
            DB.SaveChanges();
            return "FriendAdded";
        }
        [HttpPost]
        [Authorize]
        public string RemoveFriend(string FriendID)
        {
            string UserIDSession = HttpContext.Session.GetString("UserID");

            /*In Case Session Ended*/
            if (UserIDSession == null)
                return "Session Ended";
            /*******************************************************/

            var FriendToRemove1 = DB.Friends.FirstOrDefault(f => (f.FriendID == FriendID && f.User_ID == UserIDSession));
            var FriendToRemove2 = DB.Friends.FirstOrDefault(f => (f.FriendID == UserIDSession && f.User_ID == FriendID));
            DB.Friends.Remove(FriendToRemove1);
            DB.Friends.Remove(FriendToRemove2);
            DB.SaveChanges();
            return "FriendRemoved";
        }
        [HttpPost]
        [Authorize]
        public IActionResult ViewAllFriends(string UserID)
        {
            string UserIDSession = HttpContext.Session.GetString("UserID");

            /*If Session Ends Redirect To Login Page*/
            if (UserIDSession == null)
                return LocalRedirect("/Identity/Account/Login");
            /*******************************************************/

            List<MyUser> AllFriends = new List<MyUser>();
            var AllFriendsIDs = DB.Friends.Where(s => s.User_ID == UserID).Select(s => s.FriendID);
            foreach (var item in AllFriendsIDs)
                AllFriends.Add(DB.Users.Where(s => s.Id == item)
                    .Select(s => s).FirstOrDefault());
            ViewBag.State = UserIDSession == UserID;
            return PartialView(AllFriends);
        }
        #endregion


    }
}