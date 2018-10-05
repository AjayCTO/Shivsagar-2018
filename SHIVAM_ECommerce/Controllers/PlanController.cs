using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SHIVAM_ECommerce.Models;
using System.Linq.Dynamic;
using SHIVAM_ECommerce.Attributes;
using SHIVAM_ECommerce.Extensions;
using SHIVAM_ECommerce.ViewModels;
namespace SHIVAM_ECommerce.Controllers
{
    [CustomAuthorize]
    public class PlanController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public PlanController()
        {



        }
        // GET: /Plan/
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

            var v = (from a in db.Plans select a);
            if (!string.IsNullOrEmpty(searchitem))
            {

                v = v.Where(b => b.Plancode.Contains(searchitem) || b.PlanName.Contains(searchitem) || b.Description.Contains(searchitem) || b.MonthlyPrice == _searchInt || b.YearlyPrice == _searchInt || b.ProductBucketCount == _searchInt || b.UserBucketCount == _searchInt);
            }
            //SORT
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                v = v.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = v.Count();
            var data = v.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data.Select(x => new { x.Id, x.PlanName, PlanFrequency = x.PlanFrequency == "1" ? "Monthly" : "Yearly", x.Plancode, x.ProductBucketCount, x.UserBucketCount, x.Description, x.IsActive, x.MonthlyPrice, x.YearlyPrice, x.TotalYearlyPrice, x.TotalMonthlyPrice }) }, JsonRequestBehavior.AllowGet);
        }


        // GET: /Plan/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Plans plans = db.Plans.Include(x=>x.Features).Where(x=>x.Id==id).FirstOrDefault();
            if (plans == null)
            {
                return HttpNotFound();
            }
            return View(plans);
        }

        [HttpPost]
        public ActionResult TogglePlan(int ID)
        {
            try
            {
                var _plan = db.Plans.Where(x => x.Id == ID).FirstOrDefault();
                _plan.IsActive = !_plan.IsActive;

                db.SaveChanges();
                return Json(new { Success = true, ex = "" });

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, ex = ex.Message.ToString() });
            }
        }

        // GET: /Plan/Create
        public ActionResult Create()
        {
            return View(GetViewBagData());
        }

        public PlanViewModel GetViewBagData()
        {
            var _Model = new PlanViewModel();
            _Model.Features = new List<PlanFeaturesViewModel>();
            var _features = db.Features.ToList();
            foreach (var item in _features)
            {
                var _obj = new PlanFeaturesViewModel();
                _obj.ID = item.Id;
                _obj.title = item.Title;
                _obj.IsSelected = false;
                _obj.price = item.Price;
                _Model.Features.Add(_obj);
            }

            return _Model;
        }


        //uniuqe planname
        [HttpPost]
        public ActionResult UniquePlanName(string Planname)
        {
            try
            {

                var _user = db.Plans.Where(a => a.PlanName == Planname).FirstOrDefault();
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
        [HttpPost]
        public ActionResult UniquePlanCode(string Plancode)
        {
            try
            {

                var _user = db.Plans.Where(a => a.Plancode == Plancode).FirstOrDefault();
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
        // POST: /Plan/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PlanViewModel plans)
        {
            if (ModelState.IsValid)
            {
                var _newplan = new Plans();
                _newplan.UpdatedDate = DateTime.Now;
                _newplan.CreatedDate = DateTime.Now;
                _newplan.IsActive = true;
                _newplan.Description = plans.Description;
                _newplan.Notes = plans.Notes;
                _newplan.IsActive = true;
                _newplan.MonthlyPrice = plans.MonthlyPrice;
                _newplan.YearlyPrice = plans.YearlyPrice;
                _newplan.TotalMonthlyPrice = plans.TotalMonthlyPrice;
                _newplan.Plancode = plans.Plancode;
                _newplan.PlanFrequency = plans.PlanFrequency;
                _newplan.PlanName = plans.PlanName;
                _newplan.ProductBucketCount = plans.ProductBucketCount;
                _newplan.Sort = 45;
                _newplan.TotalYearlyPrice = plans.TotalYearlyPrice;
                _newplan.UserBucketCount = plans.UserBucketCount;
                db.Plans.Add(_newplan);

                db.SaveChanges();
                foreach (var _item in plans.Features)
                {
                    if (_item.IsSelected)
                    {

                        var _planfeature = new PlanFeatures();
                        _planfeature.FeatureID = _item.ID;
                        _planfeature.CreatedDate = DateTime.Now;
                        _planfeature.UpdatedDate = DateTime.Now;
                        _planfeature.PlanID = _newplan.Id;
                        db.PlanFeatures.Add(_planfeature);
                    }

                }

                db.SaveChanges();
                this.AddNotification("Plan created successfully.", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }

            return View(plans);
        }

        // GET: /Plan/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Plans plans = db.Plans.Find(id);
            if (plans == null)
            {
                return HttpNotFound();
            }

            var _Model = new PlanViewModel();
            _Model.Features = new List<PlanFeaturesViewModel>();
            var _features = db.Features.ToList();

            _Model.CreatedDate = plans.CreatedDate;
            _Model.Description = plans.Description;
            _Model.Notes = plans.Notes;
            _Model.IsActive = plans.IsActive;
            _Model.MonthlyPrice = plans.MonthlyPrice;
            _Model.YearlyPrice = plans.YearlyPrice;
            _Model.TotalMonthlyPrice = plans.TotalMonthlyPrice;
            _Model.Plancode = plans.Plancode;
            _Model.PlanFrequency = plans.PlanFrequency;
            _Model.PlanName = plans.PlanName;
            _Model.ProductBucketCount = plans.ProductBucketCount;
            _Model.TotalYearlyPrice = plans.TotalYearlyPrice;
            _Model.UserBucketCount = plans.UserBucketCount;
            _Model.Sort = plans.Sort;
            _Model.Id = plans.Id;
            var _planFeatures = db.PlanFeatures.Where(x => x.PlanID == _Model.Id).ToList();
            _planFeatures = _planFeatures == null ? new List<PlanFeatures>() : _planFeatures;

            foreach (var item in _features)
            {
                var _isExisting = _planFeatures.Where(x => x.FeatureID == item.Id).FirstOrDefault();
                var _obj = new PlanFeaturesViewModel();
                _obj.ID = item.Id;
                _obj.title = item.Title;
                _obj.IsSelected = _isExisting == null ? false : true;
                _obj.price = item.Price;
                _Model.Features.Add(_obj);
            }
            return View(_Model);
        }

        // POST: /Plan/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PlanViewModel plans)
        {
            if (ModelState.IsValid)
            {
                var _newplan = new Plans();
                _newplan.UpdatedDate = DateTime.Now;
                _newplan.CreatedDate = plans.CreatedDate;
                _newplan.Description = plans.Description;
                _newplan.Notes = plans.Notes;
                _newplan.IsActive = plans.IsActive;
                _newplan.MonthlyPrice = plans.MonthlyPrice;
                _newplan.YearlyPrice = plans.YearlyPrice;
                _newplan.TotalMonthlyPrice = plans.TotalMonthlyPrice;
                _newplan.Plancode = plans.Plancode;
                _newplan.PlanFrequency = plans.PlanFrequency;
                _newplan.PlanName = plans.PlanName;
                _newplan.ProductBucketCount = plans.ProductBucketCount;
                _newplan.TotalYearlyPrice = plans.TotalYearlyPrice;
                _newplan.UserBucketCount = plans.UserBucketCount;
                _newplan.Sort = plans.Sort;
                _newplan.UpdatedDate = DateTime.Now;
                _newplan.Id = plans.Id;
                db.Entry(_newplan).State = EntityState.Modified;
                db.SaveChanges();
                var _planFeatures = db.PlanFeatures.Where(x => x.PlanID == _newplan.Id).ToList();
                _planFeatures = _planFeatures == null ? new List<PlanFeatures>() : _planFeatures;
                foreach (var _item in plans.Features)
                {
                    var _isExisting = _planFeatures.Where(x => x.FeatureID == _item.ID).FirstOrDefault();
                    if (_item.IsSelected && _isExisting == null)
                    {

                        var _planfeature = new PlanFeatures();
                        _planfeature.FeatureID = _item.ID;
                        _planfeature.CreatedDate = DateTime.Now;
                        _planfeature.UpdatedDate = DateTime.Now;
                        _planfeature.PlanID = _newplan.Id;
                        db.PlanFeatures.Add(_planfeature);
                    }
                    else if (_isExisting != null)
                    {
                        if (!_item.IsSelected)
                        {
                            db.PlanFeatures.Remove(_isExisting);

                        }
                    }

                }

                db.SaveChanges();
                this.AddNotification("Plan updated successfully.", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }
            return View(plans);
        }



        // POST: /Plan/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {

            try
            {
                Plans plans = db.Plans.Find(id);
                var _suppliers = db.Suppliers.Where(x => x.PlanID == plans.Id).Count();
                if (_suppliers == 0)
                {

                    db.Plans.Remove(plans);
                    db.SaveChanges();

                    return Json(new { Success = true, ex = "" });
                }
                else
                {
                    return Json(new { Success = false, ex = "This Plan is currently in use" });
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
