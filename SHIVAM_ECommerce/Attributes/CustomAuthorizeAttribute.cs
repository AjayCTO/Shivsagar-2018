using SHIVAM_ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using SHIVAM_ECommerce.ViewModels;
using System.Web.Caching;
using System.Collections;
namespace SHIVAM_ECommerce.Attributes
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            string cookieName = FormsAuthentication.FormsCookieName;

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult(FormsAuthentication.LoginUrl);
                return;
            }

            var user = filterContext.HttpContext.User as ClaimsPrincipal;
            var _commonmethods = new CommonMethods();
            var _TokenData = _commonmethods.GetCurrentUser();
            _TokenData.IsSuperAdmin = true;

            if (!user.IsInRole("superadmin"))
            {
                _TokenData.IsSuperAdmin = false;

                if (_TokenData != null)
                {
                    if (_TokenData.PlanEndDate.Date > DateTime.Now.Date)
                    {

                        HttpContext.Current.Session["CurrentUserContext"] = _TokenData;
                        ManageCache(_TokenData.UserID);
                        ManageSession();
                        base.OnAuthorization(filterContext);
                    }
                    else
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "area", "" }, { "controller", "Home" }, { "action", "PlanExpire" } });
                    }
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "area", "" }, { "controller", "Home" }, { "action", "Error" } });
                }
            }
            else
            {
                HttpContext.Current.Session["CurrentUserContext"] = _TokenData;
                ManageCache(_TokenData.UserID);
                ManageSession();
                base.OnAuthorization(filterContext);
            }



        }


        private void ManageCache(string UserID)
        {
            var _isNull = false;
            if (HttpContext.Current.Cache["UserClaims"] == null && HttpContext.Current.Session["UserClaims"] == null)
            {
                _isNull = true;
            }
            else if (HttpContext.Current.Cache["UserClaims"] != null && HttpContext.Current.Session["UserClaims"] == null)
            {
                _isNull = true;
            }
            else if (HttpContext.Current.Cache["UserClaims"] == null && HttpContext.Current.Session["UserClaims"] != null)
            {
                _isNull = true;
            }
            else
            {
                var _cacheData = HttpContext.Current.Cache["UserClaims"] as List<ClaimsViewModel>;
                var _SessionData = HttpContext.Current.Session["UserClaims"] as List<ClaimsViewModel>;
                if (_cacheData == null || _SessionData == null || _cacheData.Count() == 0 || _SessionData.Count() == 0)
                {
                    _isNull = true;
                }

            }

            if (_isNull == true)
            {
                // if (HttpContext.Current.Cache["UserClaims"] == null || HttpContext.Current.Session["UserClaims"] == null)

                ApplicationDbContext db = new ApplicationDbContext();
                var userClaims = db.AspNetUserClaims.Include(x => x.claims).Where(x => x.User.Id == UserID).ToList();

                var results = userClaims.GroupBy(
    p => p.claims.ClaimGroup,
    p => p.claims,
    (key, g) => new ClaimsViewModel { Group = key, AllClaims = g.ToList() }).ToList();

                results = GetSplittedData(results);

                HttpContext.Current.Cache["UserClaims"] = userClaims;
                HttpContext.Current.Session["UserClaims"] = results;

                var CanSeeCategoryURL = userClaims.FirstOrDefault(x => x.User.Id == UserID && x.ClaimValue == "URL:/Category" && x.IsActive == true);
                if (CanSeeCategoryURL != null)
                {
                    HttpContext.Current.Cache["CanSeeCategoryURL"] = CanSeeCategoryURL.ClaimValue;
                }
                var CanSeeSupplierURL = userClaims.FirstOrDefault(x => x.User.Id == UserID && x.ClaimValue == "URL:/Supplier" && x.IsActive == true);
                if (CanSeeSupplierURL != null)
                {
                    HttpContext.Current.Cache["CanSeeSupplierURL"] = CanSeeSupplierURL.ClaimValue;
                }
                var CanSeePlanURL = userClaims.FirstOrDefault(x => x.User.Id == UserID && x.ClaimValue == "URL:/Plan" && x.IsActive == true);
                if (CanSeePlanURL != null)
                {
                    HttpContext.Current.Cache["CanSeePlanURL"] = CanSeePlanURL.ClaimValue;
                }
                var CanSeeManufacturerURL = userClaims.FirstOrDefault(x => x.User.Id == UserID && x.ClaimValue == "URL:/Manufacturers" && x.IsActive == true);
                if (CanSeeManufacturerURL != null)
                {
                    HttpContext.Current.Cache["CanSeeManufacturerURL"] = CanSeeManufacturerURL.ClaimValue;
                }
                var CanSeeProductURL = userClaims.FirstOrDefault(x => x.User.Id == UserID && x.ClaimValue == "URL:/Product/GetAllProducts" && x.IsActive == true);
                if (CanSeeProductURL != null)
                {
                    HttpContext.Current.Cache["CanSeeProductURL"] = CanSeeProductURL.ClaimValue;
                }
                var CanSeeProductStatusURL = userClaims.FirstOrDefault(x => x.User.Id == UserID && x.ClaimValue == "URL:/ProductStatus" && x.IsActive == true);
                if (CanSeeProductStatusURL != null)
                {
                    HttpContext.Current.Cache["CanSeeProductStatusURL"] = CanSeeProductStatusURL.ClaimValue;
                }
                var CanSeeUOMURL = userClaims.FirstOrDefault(x => x.User.Id == UserID && x.ClaimValue == "URL:/UnitOfMeasures" && x.IsActive == true);
                if (CanSeeUOMURL != null)
                {
                    HttpContext.Current.Cache["CanSeeUOMURL"] = CanSeeUOMURL.ClaimValue;
                }
                var CanSeeProductAttrURL = userClaims.FirstOrDefault(x => x.User.Id == UserID && x.ClaimValue == "URL:/ProductAttributes" && x.IsActive == true);
                if (CanSeeProductAttrURL != null)
                {
                    HttpContext.Current.Cache["CanSeeProductAttrURL"] = CanSeeProductAttrURL.ClaimValue;
                }
                var CanSeeProductAttrSupplierURL = userClaims.FirstOrDefault(x => x.User.Id == UserID && x.ClaimValue == "URL:/ProductAttributes/AddAttributesForSupplier" && x.IsActive == true);
                if (CanSeeProductAttrSupplierURL != null)
                {
                    HttpContext.Current.Cache["CanSeeProductAttrSupplierURL"] = CanSeeProductAttrSupplierURL.ClaimValue;
                }
                var CanSeeCustomerURL = userClaims.FirstOrDefault(x => x.User.Id == UserID && x.ClaimValue == "URL:/Customer" && x.IsActive == true);
                if (CanSeeCustomerURL != null)
                {
                    HttpContext.Current.Cache["CanSeeCustomerURL"] = CanSeeCustomerURL.ClaimValue;
                }
                var CanSeeOrderURL = userClaims.FirstOrDefault(x => x.User.Id == UserID && x.ClaimValue == "URL:/Order" && x.IsActive == true);
                if (CanSeeOrderURL != null)
                {
                    HttpContext.Current.Cache["CanSeeOrderURL"] = CanSeeOrderURL.ClaimValue;
                }
                var CanSeeEmailRecordURL = userClaims.FirstOrDefault(x => x.User.Id == UserID && x.ClaimValue == "URL:/EmailRecords" && x.IsActive == true);
                if (CanSeeEmailRecordURL != null)
                {
                    HttpContext.Current.Cache["CanSeeEmailRecordURL"] = CanSeeEmailRecordURL.ClaimValue;
                }
                var CanSeeManageAccountURL = userClaims.FirstOrDefault(x => x.User.Id == UserID && x.ClaimValue == "URL:/Account/Manage" && x.IsActive == true);
                if (CanSeeManageAccountURL != null)
                {
                    HttpContext.Current.Cache["CanSeeManageAccountURL"] = CanSeeManageAccountURL.ClaimValue;
                }
                var CanSeeSupplierUserURL = userClaims.FirstOrDefault(x => x.User.Id == UserID && x.ClaimValue == "URL:/SupplierUser" && x.IsActive == true);
                if (CanSeeSupplierUserURL != null)
                {
                    HttpContext.Current.Cache["CanSeeSupplierUserURL"] = CanSeeSupplierUserURL.ClaimValue;
                }
            }
        }

        private List<ClaimsViewModel> GetSplittedData(List<ClaimsViewModel> Claims)
        {
            if (Claims != null)
            {

                foreach (var _item in Claims.ToList())
                {
                    foreach (var _childitem in _item.AllClaims.ToList())
                    {
                        var _stringArray = _childitem.ClaimValue.Split('/');
                        if (_stringArray != null)
                        {

                            if (_stringArray.Length > 2)
                            { _childitem.ClaimValue = _stringArray[1] + "/" + _stringArray[2]; }
                            else { _childitem.ClaimValue = _stringArray[1]; }

                        }
                    }
                }
            }

            return Claims;

        }

        private void ManageSession()
        {
            HttpContext.Current.Session["CanSeeCategoryURL"] = HttpContext.Current.Cache["CanSeeCategoryURL"] != null ? HttpContext.Current.Cache["CanSeeCategoryURL"].ToString() : "";
            HttpContext.Current.Session["CanSeeSupplierURL"] = HttpContext.Current.Cache["CanSeeSupplierURL"] != null ? HttpContext.Current.Cache["CanSeeSupplierURL"].ToString() : "";
            HttpContext.Current.Session["CanSeePlanURL"] = HttpContext.Current.Cache["CanSeePlanURL"] != null ? HttpContext.Current.Cache["CanSeePlanURL"].ToString() : "";
            HttpContext.Current.Session["CanSeeManufacturerURL"] = HttpContext.Current.Cache["CanSeeManufacturerURL"] != null ? HttpContext.Current.Cache["CanSeeManufacturerURL"].ToString() : "";
            HttpContext.Current.Session["CanSeeProductURL"] = HttpContext.Current.Cache["CanSeeProductURL"] != null ? HttpContext.Current.Cache["CanSeeProductURL"].ToString() : "";
            HttpContext.Current.Session["CanSeeProductStatusURL"] = HttpContext.Current.Cache["CanSeeProductStatusURL"] != null ? HttpContext.Current.Cache["CanSeeProductStatusURL"].ToString() : "";
            HttpContext.Current.Session["CanSeeUOMURL"] = HttpContext.Current.Cache["CanSeeUOMURL"] != null ? HttpContext.Current.Cache["CanSeeUOMURL"].ToString() : "";
            HttpContext.Current.Session["CanSeeProductAttrURL"] = HttpContext.Current.Cache["CanSeeProductAttrURL"] != null ? HttpContext.Current.Cache["CanSeeProductAttrURL"].ToString() : "";
            HttpContext.Current.Session["CanSeeProductAttrSupplierURL"] = HttpContext.Current.Cache["CanSeeProductAttrSupplierURL"] != null ? HttpContext.Current.Cache["CanSeeProductAttrSupplierURL"].ToString() : "";
            HttpContext.Current.Session["CanSeeCustomerURL"] = HttpContext.Current.Cache["CanSeeCustomerURL"] != null ? HttpContext.Current.Cache["CanSeeCustomerURL"].ToString() : "";
            HttpContext.Current.Session["CanSeeOrderURL"] = HttpContext.Current.Cache["CanSeeOrderURL"] != null ? HttpContext.Current.Cache["CanSeeOrderURL"].ToString() : "";
            HttpContext.Current.Session["CanSeeEmailRecordURL"] = HttpContext.Current.Cache["CanSeeEmailRecordURL"] != null ? HttpContext.Current.Cache["CanSeeEmailRecordURL"].ToString() : "";
            HttpContext.Current.Session["CanSeeManageAccountURL"] = HttpContext.Current.Cache["CanSeeManageAccountURL"] != null ? HttpContext.Current.Cache["CanSeeManageAccountURL"].ToString() : "";
            HttpContext.Current.Session["CanSeeSupplierUserURL"] = HttpContext.Current.Cache["CanSeeSupplierUserURL"] != null ? HttpContext.Current.Cache["CanSeeSupplierUserURL"].ToString() : "";
        }


        private void RemoveCache()
        {
            List<string> keys = new List<string>();

            Cache cache = new Cache();
            IDictionaryEnumerator enumerator = cache.GetEnumerator();

            while (enumerator.MoveNext())
                keys.Add(enumerator.Key.ToString());

            for (int i = 0; i < keys.Count; i++)
                cache.Remove(keys[i]);
        }
    }
}