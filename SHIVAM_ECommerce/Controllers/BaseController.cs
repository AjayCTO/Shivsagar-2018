using SHIVAM_ECommerce.Models;
using SHIVAM_ECommerce.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SHIVAM_ECommerce.Controllers
{
    public class BaseController : Controller
    {
        //
        // GET: /Base/

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
        }
        private CurrentUserContext _user = null;

        public CurrentUserContext CurrentUserData
        {
            get
            {
                if (_user == null)
                {
                    _user = Session["CurrentUserContext"] as CurrentUserContext;
                }
                return _user;
            }
        }


    }
}