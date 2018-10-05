using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Linq.Dynamic;
using System.Web.Mvc;
using SHIVAM_ECommerce.Models;
using SHIVAM_ECommerce.Repository;
using SHIVAM_ECommerce.Extensions;
using System.IO;
namespace SHIVAM_ECommerce.Controllers
{
    public class CategoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private IRepository<Category> _repository = null;
        public CategoryController()
        {
            this._repository = new Repository<Category>();
        }
        // GET: /Category/
        public ActionResult Index()
        {
            return View();
        }



        public JsonResult AllCategories()
        {
            var categories = _repository.GetAll().Where(p => p.IsActive == true).Select(p => new { Name = p.CategoryName, Id = p.Id });
            return Json(categories.ToList(), JsonRequestBehavior.AllowGet);

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

                v = v.Where(b => b.CategoryName.ToLower().Contains(searchitem.ToLower()));
            }
            //SORT
            if (!(string.IsNullOrEmpty(sortColumn)||sortColumn=="" && string.IsNullOrEmpty(sortColumnDir)))
            {
                v = v.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = v.Count();
            var data = v.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data.Select(x => new { x.Id, x.CategoryImage, x.CategoryName }) }, JsonRequestBehavior.AllowGet);
        }

        // GET: /Category/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = _repository.GetById(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: /Category/Create
        public ActionResult Create()
        {
            ViewBag.ParentCategory = new SelectList(db.Cateogries, "Id", "CategoryName");
            return View();
        }

        // POST: /Category/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CategoryName,IsActive,ParentCategory,CategoryImage,CreatedDate,UpdatedDate,Sort,Description,Notes,IsTopCategory")] Category category, HttpPostedFileBase file)
        {
            category.CreatedDate = DateTime.Now;
            category.UpdatedDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(
                                           Server.MapPath("~/CategoryImage"), pic);
                    // file is uploaded
                    file.SaveAs(path);

                    category.CategoryImage = pic;

                    // save the image path path to the database or you can send image 
                    // directly to database
                    // in-case if you want to store byte[] ie. for DB
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }
                }


                _repository.Insert(category);
                _repository.Save();
                this.AddNotification("Category Created successfully.", NotificationType.SUCCESS);
                return RedirectToAction("Index","Category");
            }

            ViewBag.ParentCategory = new SelectList(db.Cateogries, "Id", "CategoryName", category.ParentCategory);

            return View(category);
        }

        // GET: /Category/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = _repository.GetById(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            ViewBag.ParentCategory = new SelectList(db.Cateogries, "Id", "CategoryName", category.ParentCategory);
            return View(category);
        }

        // POST: /Category/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CategoryName,IsActive,ParentCategory,CategoryImage,CreatedDate,UpdatedDate,Sort,Description,Notes,IsTopCategory")] Category category, HttpPostedFileBase file)
        {
            category.CreatedDate = DateTime.Now;
            category.UpdatedDate = DateTime.Now;
            if (ModelState.IsValid)
            {

                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(
                                           Server.MapPath("~/CategoryImage"), pic);
                    // file is uploaded
                    file.SaveAs(path);

                    category.CategoryImage = pic;

                    // save the image path path to the database or you can send image 
                    // directly to database
                    // in-case if you want to store byte[] ie. for DB
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }
                }


                _repository.Update(category);
                _repository.Save();
                this.AddNotification("Category Updated successfully.", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }

            ViewBag.ParentCategory = new SelectList(db.Cateogries, "Id", "CategoryName", category.ParentCategory);

            return View(category);
        }


        [HttpPost]
        public ActionResult UniqueCategory(string CategoryName)
        {
            try
            {

                var _user = db.Cateogries.Where(a => a.CategoryName == CategoryName).FirstOrDefault();
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









        // POST: /Category/Delete/5

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var _products = db.Products.Where(x => x.CateogryID == id).Count();
                if (_products == 0)
                {

                    _repository.Delete(id);
                    _repository.Save();
                    return Json(new { Success = true, ex = "" });
                }
                else
                {
                    return Json(new { Success = false, ex = "This Category Associated with some product, unable to delete this." });
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
