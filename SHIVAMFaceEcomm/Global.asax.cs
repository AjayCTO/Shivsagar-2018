using SHIVAMFaceEcomm.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SHIVAMFaceEcomm
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static string GlobalImageAssetUrl;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalImageAssetUrl = System.Configuration.ConfigurationManager.AppSettings["AssetsImageURL"];
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            BundleConfig.RegisterBundles(BundleTable.Bundles);

        }
    }
}
