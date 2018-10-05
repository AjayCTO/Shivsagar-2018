using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Linq.Dynamic;
using System.Web.Mvc;
using SHIVAM_ECommerce.Models;
using SHIVAM_ECommerce.Repository;
using SHIVAM_ECommerce.Extensions;
using System.IO;
using SHIVAM_ECommerce.Attributes;
namespace SHIVAM_ECommerce.Controllers
{
    [CustomAuthorize]
    public class ProductStatusController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private IRepository<ProductStatus> _repository = null;

        public ProductStatusController()
        {
            this._repository = new Repository<ProductStatus>();
        }
        // GET: /ProductStatus/
        public async Task<ActionResult> Index()
        {
            return View();
        }



        public ActionResult LoadData()
        {

            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var searchitem = Request["search[value]"];
            //Find Order Column
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            // dc.Configuration.LazyLoadingEnabled = false; // if your table is relational, contain foreign key
            var v = (from a in _repository.GetAll() select a);
            if (!string.IsNullOrEmpty(searchitem))
            {

                v = v.Where(b => b.Name.ToLower().Contains(searchitem.ToLower()));
            }
            //SORT
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                v = v.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = v.Count();
            var data = v.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data.Select(x => new { x.Id, x.Name, x.IsActive }) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UniqueStatus(string Status)
        {
            try
            {

                var _user = db.ProductStatus.Where(a => a.Name == Status).FirstOrDefault();
                if (_user != null)
                {
                    return Json(new { Success = true, ex = "", IsAlreadyExist = true });
                }
                return Json(new { Success = true, ex = "", IsAlreadyExist = false });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, ex = ex.Message.ToString(), IsAlreadyExist = false });
            }
        }







        // GET: /ProductStatus/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductStatus productstatus = await db.ProductStatus.FindAsync(id);
            if (productstatus == null)
            {
                return HttpNotFound();
            }
            return View(productstatus);
        }

        // GET: /ProductStatus/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: /ProductStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,IsActive")] ProductStatus productstatus)
        {
            if (ModelState.IsValid)
            {
                //_repository.Insert(productstatus);
                //_repository.Save();
                productstatus.CreatedDate = DateTime.Now;
                productstatus.UpdatedDate = DateTime.Now;

                db.ProductStatus.Add(productstatus);
                await db.SaveChangesAsync();
                this.AddNotification("Product Status Created successfully.", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }

            return View(productstatus);
        }

        // GET: /ProductStatus/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductStatus productstatus = await db.ProductStatus.FindAsync(id);
            if (productstatus == null)
            {
               // return HttpNotFound();
                return View(productstatus);
            }
            return View(productstatus);
        }

        // POST: /ProductStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,IsActive,CreatedDate")] ProductStatus productstatus)
        {
            if (ModelState.IsValid)
            {
                productstatus.UpdatedDate = DateTime.Now;
                db.Entry(productstatus).State = EntityState.Modified;
                await db.SaveChangesAsync();
                this.AddNotification("Product Status updated successfully.", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }
            return View(productstatus);
        }

        // GET: /ProductStatus/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductStatus productstatus = await db.ProductStatus.FindAsync(id);
            if (productstatus == null)
            {
                return HttpNotFound();
            }
            return View(productstatus);
        }

        // POST: /ProductStatus/Delete/5
        [HttpPost]

        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var _products = db.ProductAttributeWithQuantity.Where(x => x.StatusId == id).Count();
                if (_products == 0)
                {


                    ProductStatus productstatus = await db.ProductStatus.FindAsync(id);
                    db.ProductStatus.Remove(productstatus);
                    await db.SaveChangesAsync();
                    return Json(new { Success = true, ex = "" });
                }
                else
                {
                    return Json(new { Success = false, ex = "This status Associated with some product, unable to delete this." });
                }
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
