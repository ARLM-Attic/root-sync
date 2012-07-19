using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Principal;
using www.Models;
using System.Web.Security;
using System.Threading;


namespace www.Controllers
{
    public class accountController : Controller
    {
        //
        // GET: /profile/

        public ActionResult Index(string path)
        {
            if (string.IsNullOrEmpty(path)) path = "";
            return Redirect(string.Format("{0}://{1}/files/{2}", Request.Url.Scheme, Request.Url.Authority, path));
        }


        public ActionResult Edit()
        {
            Int32 userId = Int32.Parse(User.Identity.Name);

            accountModel model = DataAccess.DAL.retAccount(userId);

            return View(model);
        }



        public ActionResult Update(accountModel model)
        {
            Int32 userId = Int32.Parse(User.Identity.Name);

            if (!DataAccess.DAL.updateAccount(userId, model))
            {
                ModelState.AddModelError("", "Failure to save profile!");
            }
            else
            {
                ViewBag.Message = "Successfully Updated!";
            }


            return View("Edit");
        }

















        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }



        [HttpGet]
        public ActionResult SignIn()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView("_signin");
            }
            return View();
        }




        [HttpPost]
        public ActionResult SignIn(signinModel model)
        {
            Thread.Sleep(500); // Fake processing time

            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                    return PartialView("_signin", model);

                return View(model);
            }

            //if (model.Username == "daniel" && model.Password == "1234") //simulate data store call where username/password exists
            Int32 UserID = DataAccess.DAL.UserIsValid(model.Username, model.Password);

            if (UserID != -1)
            {
                FormsAuthentication.SetAuthCookie(UserID.ToString(), true); //false = cookie destroyed by closing browser window

                if (!Request.Path.ToLower().Contains("signin"))
                    return JavaScript("location.reload(true)");
                else return JavaScript("window.location = '" + Request.Url.ToString().ToLower().Replace("/signin", "") + "'");
            }
            else
            {
                ModelState.AddModelError("Password", "Invalid username or password");
                return Json(new object[] { true, this.RenderPartialViewToString("_failure", model) });
            }
        }









        [HttpGet]
        public ActionResult Register()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView("_register");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Register(accountModel model)
        {
            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                    return PartialView("_register", model);

                return View(model);
            }


            Int32 userId = DataAccess.DAL.RegisterAccount(model.First, model.Last, model.Username, model.Password);

            if (userId != -1)
            {
                FormsAuthentication.SetAuthCookie(userId.ToString(), true); //false = cookie destroyed by closing browser window

                //-----------------
                //FILES - create ftp folder if it doesn't exist.
                string FTPPath = www.Core.Utility.storagePath() + userId + "/";
                if (!System.IO.Directory.Exists(FTPPath)) System.IO.Directory.CreateDirectory(FTPPath);
                //-----------------



                if (!Request.Path.ToLower().Contains("signin"))
                    return JavaScript("location.reload(true)");
                else return JavaScript("window.location = '" + Request.Url.ToString().ToLower().Replace("/signin", "") + "'");

            }

            return View();
        }








    }
}
