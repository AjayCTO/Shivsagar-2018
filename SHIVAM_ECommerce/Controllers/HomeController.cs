using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SHIVAM_ECommerce.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using ExportImplementation;
using System.Diagnostics;
using System.IO;
using SHIVAM_ECommerce.Repository;
using SHIVAM_ECommerce.Attributes;
using System.Collections;
using System.Web.Caching;
using SHIVAM_ECommerce.ViewModels;
using SHIVAM_ECommerce.Extensions;

namespace SHIVAM_ECommerce.Controllers
{
    public class HomeController : BaseController
    {


        private ApplicationDbContext db = new ApplicationDbContext();
        [CustomAuthorize]
        public ActionResult Index()
        {
            if (CurrentUserData.IsSuperAdmin == true)
            {

                ViewBag.SupplierCount = db.Suppliers.Count();
                ViewBag.CustomerCount = db.Customers.Count();
                ViewBag.OrdersCount = db.Orders.Count();
            }
            else
            {

                var _orderIds = db.OrderItems.Where(x => x.SupplierID == CurrentUserData.SupplierID).Select(x => x.Orders_Id);
                var _customerIds = _orderIds != null ? db.Orders.Include(o => o.customer).Where(x => _orderIds.Contains(x.Id)).Select(x => x.CustomerID) : null;

                var customers = _orderIds != null ? db.Customers.Where(x => _customerIds.Contains(x.Id)).Count() : 0;

                var _orderIdss = db.OrderItems.Where(x => x.SupplierID == CurrentUserData.SupplierID).Select(x => x.Orders_Id);
                var orders = _orderIdss != null ? db.Orders.Include(o => o.customer).Where(x => _orderIds.Contains(x.Id)).Count() : 0;
                var _productids = db.Products.Where(x => x.SupplierID == CurrentUserData.SupplierID).Select(x => x.Id);


                ViewBag.ProductCount = _productids != null ? db.ProductAttributeWithQuantity.Where(x => _productids.Contains(x.ProductId)).Count() : 0;
                ViewBag.CustomerCount = customers;
                ViewBag.OrdersCount = orders;

            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }


        //public ActionResult Error()
        //{
        //    ViewBag.Message = "You are not authorized to access this page, please contact administrator.";

        //    return View();
        //}

        [CustomAuthorize]
        public ActionResult Error()
        {
            ViewBag.Message = Session["LastException"] as Exception;
            return View();
        }

        public ActionResult PlanExpire()
        {
            ViewBag.Message = "You are selected plan has been expired, please contact administrator to update your plan.";

            return View();
        }


        [Authorize(Roles = "Admin")]
        [ClaimsAuthorize("Admin", "Contact")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Profile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (id == -1)
            {
                return RedirectToAction("Adminprofile");
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            ViewBag.PlanID = new SelectList(db.Plans, "Id", "PlanName", supplier.PlanID);
            // ViewBag.UserID = new SelectList(db.IdentityUsers, "Id", "UserName", supplier.UserID);
            return View(supplier);
        }


        public ActionResult Adminprofile()
        {
            AdminProfile adminprofile = db.Adminprofile.FirstOrDefault();

            return View(adminprofile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Adminprofile([Bind(Include = "Id,CompanyName,FirstName,LastName,Title,Address,City,State,PostalCode,Country,Phone,Email,Logo,CreatedDate")] AdminProfile adminprofile, HttpPostedFileBase file)
        {
             var userId=User.Identity.GetUserId();
            adminprofile.UpdatedDate = DateTime.Now;
            adminprofile.UserId = userId;

            if (ModelState.IsValid)
            {

                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(
                                           Server.MapPath("~/SupplierImage"), pic);
                    // file is uploaded
                    file.SaveAs(path);

                    adminprofile.Logo = pic;

                    // save the image path path to the database or you can send image 
                    // directly to database
                    // in-case if you want to store byte[] ie. for DB
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }
                }

                db.Entry(adminprofile).State = EntityState.Modified;

                db.SaveChanges();
                this.AddNotification("Profile Update successfully.", NotificationType.SUCCESS);
            }
            return View(adminprofile);
           
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Profile([Bind(Include = "Id,CompanyName,FirstName,LastName,Title,Address1,Address2,City,State,PostalCode,Country,Phone,Email,URL,Logo,SupplierType,UserID,PlanID,PlanStartDate,PlanEndDate,UserName,Password,ParentSupplierID,CreatedDate,User_Name,ProductCount,UserCount,RegisteredByID")] Supplier supplier, HttpPostedFileBase file)
        {
            //supplier.UserName = "testtestsetetsetst";
            //supplier.Password = "testtestteststeest";
            supplier.UpdatedDate = DateTime.Now;
            supplier.Sort = 33;


            if (ModelState.IsValid)
            {

                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(
                                           Server.MapPath("~/SupplierImage"), pic);
                    // file is uploaded
                    file.SaveAs(path);

                    supplier.Logo = pic;

                    // save the image path path to the database or you can send image 
                    // directly to database
                    // in-case if you want to store byte[] ie. for DB
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }
                }




                db.Entry(supplier).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.PlanID = new SelectList(db.Plans, "Id", "PlanName", supplier.PlanID);
                this.AddNotification("Profile Update successfully.", NotificationType.SUCCESS);
                return View(supplier);
            }
            ViewBag.PlanID = new SelectList(db.Plans, "Id", "PlanName", supplier.PlanID);
            return View(supplier);
        }


    }
}