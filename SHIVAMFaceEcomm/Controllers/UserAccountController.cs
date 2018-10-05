using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SHIVAMFaceEcomm.Controllers
{
    public class UserAccountController : Controller
    {
        //
        // GET: /UserAccount/
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult MyAccount(Guid CustomerId)
        {
            return View();
        }

        public ActionResult Setting(Guid CustomerId)
        {
            return View();
        }
	}
}