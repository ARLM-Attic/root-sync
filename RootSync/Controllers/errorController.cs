using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace www.Controllers
{
    public class errorController : Controller
    {

        public ActionResult HttpError()
        {
            return View("Error");
        }

        public ActionResult Http404()
        {
            Exception ex = null;

            try
            {
                ex = (Exception)HttpContext.Application[Request.UserHostAddress.ToString()];
            }
            catch { }
                                    
            return View("Error");
        }

        // (optional) Redirect to home when /Error is navigated to directly
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }
    


    }
}
