using SHIVAM_ECommerce.Attributes;
using SHIVAM_ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using System.Net;
using SHIVAM_ECommerce.Extensions;
using System.Data.Entity;
namespace SHIVAM_ECommerce.Controllers
{

    [CustomAuthorize]
    public class FeaturesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //
        // GET: /Features/
        public ActionResult Index()
        {
            return View();
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

            var v = (from a in db.Features select a);
            if (!string.IsNullOrEmpty(searchitem))
            {

                v = v.Where(b => b.Description.Contains(searchitem) || b.Notes.Contains(searchitem) || b.Title.Contains(searchitem) || b.Price == _searchInt);
            }
            //SORT
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                v = v.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = v.Count();
            var data = v.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data.Select(x => new { x.Id, x.Title, x.Price, x.Description }) }, JsonRequestBehavior.AllowGet);
        }

        // GET: /Featured/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Features features = await db.Features.FindAsync(id);
            if (features == null)
            {
                return HttpNotFound();
            }
            return View(features);
        }

        // GET: /Featured/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Featured/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,Price,CreatedDate,UpdatedDate,Sort,Description,Notes")] Features features)
        {
            if (ModelState.IsValid)
            {
                features.CreatedDate = DateTime.Now;
                features.UpdatedDate = DateTime.Now;
                db.Features.Add(features);
                await db.SaveChangesAsync();
                this.AddNotification("Featured Added successfully.", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }

            return View(features);
        }

        // GET: /Featured/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Features features = await db.Features.FindAsync(id);
            if (features == null)
            {
                return HttpNotFound();
            }
            return View(features);
        }

        // POST: /Featured/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Price,CreatedDate,UpdatedDate,Sort,Description,Notes")] Features features)
        {
            if (ModelState.IsValid)
            {
                features.UpdatedDate = DateTime.Now;
                db.Entry(features).State = EntityState.Modified;
                await db.SaveChangesAsync();
                this.AddNotification("Featured Updated successfully.", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }
            return View(features);
        }



        // POST: /Featured/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {

                var _planFeatures = db.PlanFeatures.Where(x => x.FeatureID == id);
                if (!_planFeatures.Any())
                {

                    Features features = await db.Features.FindAsync(id);
                    db.Features.Remove(features);
                    await db.SaveChangesAsync();
                    return Json(new { Success = true, ex = "" });
                }
                else
                {
                    return Json(new { Success = false, ex = "This feature is currently in use for existing plan." });
                }
            }
            catch (Exception ex)
            {

                return Json(new { Success = false, ex = ex.Message.ToString() });
            }
        }
    }
}