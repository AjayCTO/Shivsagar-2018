using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SHIVAM_ECommerce.Models;
using SHIVAM_ECommerce.Attributes;
using System.Linq.Dynamic;
using SHIVAM_ECommerce.Extensions;
using System.IO;
namespace SHIVAM_ECommerce.Controllers
{
    [CustomAuthorize]
    public class FeaturedSupplierController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /FeaturedSupplier/
        public ActionResult Index()
        {
            var featuredsuppliers = db.FeaturedSuppliers.Include(f => f.Supplier);
            return View(featuredsuppliers.ToList());
        }
        public ActionResult LoadData()
        {

            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var searchitem = Request["search[value]"];
            int _searchInt = -1;
            if (int.TryParse(searchitem, out _searchInt))
            {
                _searchInt = int.Parse(searchitem);
            }
            else
            {
                _searchInt = -1;
            }
            //Find Order Column
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var v = (from a in db.FeaturedSuppliers select a);
            if (!string.IsNullOrEmpty(searchitem))
            {

                v = v.Where(b => b.ImagePath.Contains(searchitem) || b.OfferMessage.Contains(searchitem) || b.Description.Contains(searchitem));
            }
            //SORT
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                v = v.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = v.Count();
            var data = v.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data.Select(x => new { x.Id, x.OfferMessage, x.ImagePath, x.Description, Name = x.Supplier.FirstName + " " + x.Supplier.LastName }) }, JsonRequestBehavior.AllowGet);
        }
        // GET: /FeaturedSupplier/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeaturedSupplier featuredsupplier = db.FeaturedSuppliers.Find(id);
            if (featuredsupplier == null)
            {
                return HttpNotFound();
            }
            return View(featuredsupplier);
        }

        // GET: /FeaturedSupplier/Create
        public ActionResult Create()
        {
            ViewBag.SupplierID = new SelectList(db.Suppliers, "Id", "CompanyName");
            return View();
        }

        // POST: /FeaturedSupplier/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,SupplierID,OfferMessage,ImagePath,CreatedDate,UpdatedDate,Sort,Description,Notes")] FeaturedSupplier featuredsupplier, HttpPostedFileBase ImagePath)
        {
            if (ModelState.IsValid)
            {

                if (ImagePath != null)
                {
                    string pic = System.IO.Path.GetFileName(ImagePath.FileName);
                    string path = System.IO.Path.Combine(
                                           Server.MapPath("~/FeaturedImages"), pic);
                    // file is uploaded
                    ImagePath.SaveAs(path);

                    featuredsupplier.ImagePath = pic;


                }
                featuredsupplier.CreatedDate = DateTime.Now;
                featuredsupplier.UpdatedDate = DateTime.Now;
                db.FeaturedSuppliers.Add(featuredsupplier);
                db.SaveChanges();
                this.AddNotification("Featured supplier added successfully.", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }

            ViewBag.SupplierID = new SelectList(db.Suppliers, "Id", "CompanyName", featuredsupplier.SupplierID);
            return View(featuredsupplier);
        }

        // GET: /FeaturedSupplier/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeaturedSupplier featuredsupplier = db.FeaturedSuppliers.Find(id);
            if (featuredsupplier == null)
            {
                return HttpNotFound();
            }
            ViewBag.SupplierID = new SelectList(db.Suppliers, "Id", "CompanyName", featuredsupplier.SupplierID);
            return View(featuredsupplier);
        }

        // POST: /FeaturedSupplier/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SupplierID,OfferMessage,ImagePath,CreatedDate,UpdatedDate,Sort,Description,Notes")] FeaturedSupplier featuredsupplier, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                featuredsupplier.UpdatedDate = DateTime.Now;

                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(
                     Server.MapPath("~/FeaturedImages"), pic);
                    // file is uploaded
                    file.SaveAs(path);

                    featuredsupplier.ImagePath = pic;


                }
                db.Entry(featuredsupplier).State = EntityState.Modified;
                db.SaveChanges();
                this.AddNotification("Featured supplier updated successfully.", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }
            ViewBag.SupplierID = new SelectList(db.Suppliers, "Id", "CompanyName", featuredsupplier.SupplierID);
            return View(featuredsupplier);
        }



        // POST: /Featured/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                FeaturedSupplier featuredsupplier = db.FeaturedSuppliers.Find(id);
                db.FeaturedSuppliers.Remove(featuredsupplier);
                db.SaveChanges();
                return Json(new { Success = true, ex = "" });
            }
            catch (Exception ex)
            {

                return Json(new { Success = false, ex = ex.Message.ToString() });
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
