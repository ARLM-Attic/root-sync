using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Principal;
using www.Models;
using System.Web.Security;
using System.Threading;
using System.Configuration;
using www.DataAccess;


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
            rootsync.Business.Models.User usr = rootsync.Business.Models.User.GetUserByGUID(Guid.Parse(User.Identity.Name));
            accountModel model = accountModel.FromUser(usr);

            return View(model);
        }



        public ActionResult Update(accountModel model)
        {
            Guid guid = Guid.Parse(User.Identity.Name);

            try {
                DataAccess.DAL.UpdateAccount(guid, model);
                ViewBag.Message = "Successfully Updated!";
            } catch (Exception ex) {
                ModelState.AddModelError("", "Failure to save profile!");
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
            if (!ModelState.IsValid)
            {
                return Json(new {
                    status = "failure",
                    responseHTML = this.RenderPartialViewToString("_SignIn", model)
                });
            }

            //if (model.Username == "daniel" && model.Password == "1234") //simulate data store call where username/password exists
            Guid guid = Guid.Empty;

            try
            {
                guid = DataAccess.DAL.UserIsValid(model.Username, model.Password, true);
            }
            catch (Exception ex)
            {

            }

            if (guid != Guid.Empty)
            {
                FormsAuthentication.SetAuthCookie(guid.ToString(), true); //false = cookie destroyed by closing browser window

                return Json(new { status = "success", responseHTML = "" });

                //if (!Request.Path.ToLower().Contains("signin"))
                //    return JavaScript("location.reload(true)");
                //else return JavaScript("window.location = '" + Request.Url.ToString().ToLower().Replace("/signin", "") + "'");
            }
            else
            {
                ModelState.AddModelError("Password", "Invalid username or password");
                return Json(new {
                    status = "failure",
                    responseHTML = this.RenderPartialViewToString("_SignIn", model)
                });
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
                return Json(new {
                    status = "failure",
                    responseHTML = this.RenderPartialViewToString("_register", model)
                });
            }


            Guid guid = Guid.Empty;
            try
            {
                guid = DataAccess.DAL.RegisterAccount(model.First, model.Last, model.Username, model.Password);
            }
            catch (Exception ex) {
                if (Request.IsAjaxRequest()) {
                    return Json(new {
                        status = "error",
                        responseHTML = this.RenderPartialViewToString("~/Views/Error/AjaxError.cshtml", ex)
                    });
                }
            }

            if (guid != Guid.Empty)
            {
                FormsAuthentication.SetAuthCookie(guid.ToString(), true); //false = cookie destroyed by closing browser window

                //-----------------
                //FILES - create ftp folder if it doesn't exist.
                string FTPPath = ConfigurationManager.AppSettings["path"] + guid.ToString() + "/";
                if (!System.IO.Directory.Exists(FTPPath)) System.IO.Directory.CreateDirectory(FTPPath);
                //-----------------

                return Json(new { status = "success", responseHTML = "" });
            } else {
                if (Request.IsAjaxRequest()) {
                    return Json(new { status = "error", responseHTML = this.RenderPartialViewToString("~/Views/Error/AjaxError.cshtml", new Exception("Generic failure. UserID is -1.")) });
                }
            }

            return View();
        }








    }
}
