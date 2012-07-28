using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.ComponentModel;
using System.Web.Security;
using System.Security.Principal;
using System.Threading;
using System.Data.Entity;
using rootsync.Business.Models;




namespace www
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            //handles the browsing of files with a specified path
            routes.MapRoute(
                "Files",
                "Files/{*path}",
                new { controller = "Files", action = "Index"} // Parameter defaults
            );

            //default route is to forward to the homeController unless specified otherwise in the path
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            Database.SetInitializer<RootSyncContext>(new RootSyncCustomInitializer());
        }




        /// <summary>
        /// Gathers an error message and forwards to the errorController for processing
        /// this controller has code for short-url handling
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();  
  
            Application[HttpContext.Current.Request.UserHostAddress.ToString()] = ex;  
        } 




        

    }
}