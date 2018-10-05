﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SHIVAM_ECommerce
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Bootstrapper.Initialise();
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            //if (HttpContext.Current.Session != null)
            //{
                
            //}
            var _exception = Server.GetLastError();
            //Session["LastException"] = _exception;
            Response.Redirect("/Home/Error");

        }
    }
}