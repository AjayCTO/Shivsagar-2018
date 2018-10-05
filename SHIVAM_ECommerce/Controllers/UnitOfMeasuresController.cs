using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SHIVAM_ECommerce.Models;
using SHIVAM_ECommerce.Repository;
using SHIVAM_ECommerce.Extensions;
using System.Linq.Dynamic;
namespace SHIVAM_ECommerce.Controllers
{
    public class UnitOfMeasuresController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        private IRepository<UnitOfMeasures> _repository = null;
        public UnitOfMeasuresController()
        {
            this._repository = new Repository<UnitOfMeasures>();
        }
        // GET: /Category/
        public ActionResult Index()
        {
            return View(_repository.GetAll());
        }


        public JsonResult GetUnitOfMeasure()
        {
            var manufacturers = _repository.GetAll().Select(p => new { Name = p.UnitOfMeasuresName, Id = p.Id });
            return Json(manufacturers.ToList(), JsonRequestBehavior.AllowGet);

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

                v = v.Where(b => b.UnitOfMeasuresName.ToLower().Contains(searchitem.ToLower()));
            }
            //SORT
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
              v = v.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = v.Count();
            var data = v.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data.Select(x => new { x.Id, x.UnitOfMeasuresName }) }, JsonRequestBehavior.AllowGet);
        }

        // GET: /Category/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnitOfMeasures UM = _repository.GetById(id);
            if (UM == null)
            {
                return HttpNotFound();
            }
            return View(UM);
        }

        // GET: /Category/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Category/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UnitOfMeasuresName")] UnitOfMeasures UM)
        {
         
            if (ModelState.IsValid)
            {
                _repository.Insert(UM);
                _repository.Save();
                this.AddNotification("Unit of Measure Created successfully.", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }

            return View(UM);
        }

        // GET: /Category/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnitOfMeasures UM = _repository.GetById(id);
            if (UM == null)
            {
                return HttpNotFound();
            }
            return View(UM);
        }

          [HttpPost]
        public ActionResult UniqueUOfMName(string UOfMName)
        {
            try
            {

                var _user = db.UnitOfMeasures.Where(a => a.UnitOfMeasuresName == UOfMName).FirstOrDefault();
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




        // POST: /Category/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UnitOfMeasuresName")] UnitOfMeasures UM)
        {
           
            if (ModelState.IsValid)
            {
                _repository.Update(UM);
                _repository.Save();
                this.AddNotification("Unit of Measure Updated successfully.", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }
            return View(UM);
        }




        // GET: /Category/Delete/5
       

    

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var db=new ApplicationDbContext();
                var _products = db.Products.Where(x => x.UnitOfMeasuresId == id).Count();
                if (_products == 0)
                {

                    _repository.Delete(id);
                    _repository.Save();
                    return Json(new { Success = true, ex = "" });
                }
                else
                {
                    return Json(new { Success = false, ex = "This Unit of measure Associated with some product, unable to delete this." });
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
            }
            base.Dispose(disposing);
        }
    }
}
