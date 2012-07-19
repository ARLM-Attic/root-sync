using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Threading;
using www.Models;

namespace www.Controllers
{
    public class homeController : Controller
    {
        //
        // GET: /home/

        public ActionResult Index()
        {            
            if (Request.IsAuthenticated)
            {
                return Redirect(string.Format("{0}://{1}/files", Request.Url.Scheme, Request.Url.Authority));
            }
            else
            {
                return View("Index");
            }
        }


        

    }
}
