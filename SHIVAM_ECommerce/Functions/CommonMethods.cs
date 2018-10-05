using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using SHIVAM_ECommerce.ViewModels;
using SHIVAM_ECommerce.Controllers;
using Microsoft.AspNet.Identity;
namespace SHIVAM_ECommerce.Models
{
    public class CommonMethods
    {
        public CurrentUserContext GetCurrentUser()
        {
            var _identity = HttpContext.Current.User.Identity;

            if (!_identity.IsAuthenticated)
            {
                return null;
            }
            ApplicationDbContext db = new ApplicationDbContext();
            var _userID = HttpContext.Current.User.Identity.GetUserId();
            var _supplier = db.Suppliers.Where(x => x.UserID == _userID).FirstOrDefault();

            CurrentUserContext _user = new CurrentUserContext
            {
                UserID = _userID,
                UserName = _identity.Name,
                SupplierID = _supplier == null ? -1 : _supplier.Id,
                PlanEndDate = _supplier == null ? DateTime.Now : _supplier.PlanEndDate,
                ProductCount = _supplier == null ? 0 : _supplier.ProductCount,
                UserCount = _supplier == null ? 0 : _supplier.UserCount,
                PlanId = _supplier == null ? 0 : _supplier.PlanID,
                CompanyName = _supplier == null ? "" : _supplier.CompanyName
            };

            HttpContext.Current.Session["CurrentUserContext"] = _user;

            return _user;
        }


    }
}