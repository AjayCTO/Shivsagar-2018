using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SHIVAM_ECommerce.Models;
using System.Drawing;
using System.IO;
using SHIVAM_ECommerce.Repository;
using SHIVAM_ECommerce.ViewModels;
using SHIVAM_ECommerce.Attributes;

namespace SHIVAM_ECommerce.Controllers
{
    [CustomAuthorize]
    public class AllImagesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();




        private IRepository<ProductImages> _Imagesrepository = null;
        private IRepository<AllProductImages> _repository = null;
        public AllImagesController()
        {
            this._repository = new Repository<AllProductImages>();
            this._Imagesrepository = new Repository<ProductImages>();
        }



        // GET: /AllImages/
        [HttpGet]
        public ActionResult Index()
        {
            var supplierId = -1;

            if (Session["SelectedSupplier"] != null)
            {
                var selectedSupplier = Session["SelectedSupplier"] as Supplier;
                supplierId = selectedSupplier.Id;
            }
            else {
                supplierId = CurrentUserData.SupplierID;
            }       


            ViewBag.Suppliers = CurrentUserData.SupplierID == -1 ? db.Suppliers.ToList() : new List<Supplier>();
            return View(db.AllProductImages.Where(a => a.SupplierID == supplierId));
        }


        [HttpPost]
        public ActionResult Index(int SupplierID = 1)
        {
            var strDDLValue = -1;
            if (!String.IsNullOrEmpty(Request.Form["SupplierData1"]))
            {
                 strDDLValue = Convert.ToInt32(Request.Form["SupplierData1"]);
            }

            Session["SelectedSupplier"] = db.Suppliers.FirstOrDefault(x => x.Id == strDDLValue); 

            //TempData["SelectedSupplier"] = db.Suppliers.FirstOrDefault(x => x.Id == strDDLValue);           
            ViewBag.Suppliers = CurrentUserData.SupplierID == -1 ? db.Suppliers.ToList() : new List<Supplier>();
            return View("Index", db.AllProductImages.Where(a => a.SupplierID == strDDLValue));
        }

        public ActionResult TestMethod()
        {
            ViewBag.Suppliers = CurrentUserData.SupplierID == -1 ? db.Suppliers.ToList() : new List<Supplier>();
            return View("Index", db.AllProductImages.Where(a => a.SupplierID == 1));
        }


        public ActionResult GetImages(int SupplierID)
        {
            try
            {
                ViewBag.Suppliers = CurrentUserData.SupplierID == -1 ? db.Suppliers.ToList() : new List<Supplier>();
                var ImagesList = db.AllProductImages.Where(a => a.SupplierID == SupplierID).ToList();
                return View("Index", db.AllProductImages.Where(a => a.SupplierID == SupplierID));
               // return Json(new { Success = true, Data = ImagesList, ex = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = true, Data = "", ex = ex.Message.ToString(), JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]

        public async Task<ActionResult> DeleteAssignedimage(int[] ids)
        {
            foreach (int id in ids)
            {
                ProductImages ProductImages = await db.ProductImages.FindAsync(id);
                db.ProductImages.Remove(ProductImages);
            }
            await db.SaveChangesAsync();
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult AllImageData(ImageListViewModel model)
        {
            try
            {
                var _data = db.AllProductImages.Where(a => a.SupplierID == CurrentUserData.SupplierID);
                int _TotalCount = _data.Count();
                _data = !string.IsNullOrEmpty(model.SearchString) ? _data.Where(x => x.ImageName.Contains(model.SearchString)) : _data;
                int _rowsSkip = model.pageSize * (model.page - 1);
                var _Results = _data.OrderBy(x => x.Sort).Skip(_rowsSkip).Take(model.pageSize).ToList();

                return Json(new { Success = true, data = _Results.Select(x => new { x.Id, x.ImageName, x.ImagePath, x.Sort, x.Description }), ex = "", TotalCount = _TotalCount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, data = "", ex = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult AllImageDataSelected(ImageListViewModel model)
        {
            try
            {

                var _data = db.AllProductImages.Where(a => a.SupplierID == CurrentUserData.SupplierID);
                int _TotalCount = _data.Count();
                _data = !string.IsNullOrEmpty(model.SearchString) ? _data.Where(x => x.ImageName.Contains(model.SearchString)) : _data;
                int _rowsSkip = model.pageSize * (model.page - 1);
                var _Results = _data.OrderBy(x => x.Sort).Skip(_rowsSkip).Take(model.pageSize).ToList();

                return Json(new { Success = true, data = _Results.Select(x => new { x.Id, x.ImageName, x.ImagePath, x.Sort, x.Description }), ex = "", TotalCount = _TotalCount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, data = "", ex = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        public static void Crop(int Width, int Height, Stream streamImg, string saveFilePath)
        {
            Bitmap sourceImage = new Bitmap(streamImg);
            using (Bitmap objBitmap = new Bitmap(Width, Height))
            {
                objBitmap.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
                using (Graphics objGraphics = Graphics.FromImage(objBitmap))
                {
                    // Set the graphic format for better result cropping   
                    objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    objGraphics.DrawImage(sourceImage, 0, 0, Width, Height);

                    // Save the file path, note we use png format to support png file   
                    objBitmap.Save(saveFilePath);
                }
            }
        }





        [HttpPost]
        public ActionResult UploadImage(AllProductImages allproductimages, IEnumerable<HttpPostedFileBase> files)
        {
            var selectedSupplier = new Supplier();

            if (Session["SelectedSupplier"] != null)
            {
                selectedSupplier = Session["SelectedSupplier"] as Supplier;
            }
            else {
                selectedSupplier.Id = CurrentUserData.SupplierID;
            }
            var curentUserID = CurrentUserData.UserID;

            foreach (var file in files)
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/ProductImages/large"), fileName);
                    file.SaveAs(path);
                    var _filePathData = Guid.NewGuid().ToString() + Path.GetExtension(path);
                    path = Path.Combine(Server.MapPath("~/ProductImages/large"), _filePathData);
                    file.SaveAs(path);
                    Crop(150, 150, file.InputStream, Path.Combine(Server.MapPath("~/ProductImages/thumb/") + _filePathData));

                    allproductimages.ImageName = fileName;
                    allproductimages.ImagePath = _filePathData;
                    allproductimages.CreatedDate = DateTime.Now;
                    allproductimages.UpdatedDate = DateTime.Now;
                    allproductimages.UserID = curentUserID;
                    allproductimages.SupplierID = selectedSupplier.Id;
                    _repository.Insert(allproductimages);
                    _repository.Save();
                }
            }
            return RedirectToAction("index");
        }


        [HttpPost]
        public ActionResult SaveImages(ProductImagesAssignViewModel Data)
        {
            try
            {

                foreach (var _image in Data.Path)
                {
                    var _imageName = "";

                    _imageName = Guid.NewGuid().ToString() + ".png";

                    var _OldPath = System.Web.Hosting.HostingEnvironment.MapPath("~/ProductImages/large/");
                    var oldPathAndName = _OldPath + _image;
                    var _NewPath = System.Web.Hosting.HostingEnvironment.MapPath("~/ProductImages/");
                    var newPathAndName = _NewPath + _imageName;

                    if (System.IO.File.Exists(oldPathAndName))
                        System.IO.File.Copy(oldPathAndName, newPathAndName);


                    var _productImage = new ProductImages();
                    _productImage.ImageName = _image;
                    _productImage.ImagePath = _imageName;
                    _productImage.ProductQuantityId = Data.ProductID;
                    _productImage.CreatedDate = DateTime.Now;
                    _productImage.UpdatedDate = DateTime.Now;
                    _Imagesrepository.Insert(_productImage);
                    _Imagesrepository.Save();
                }

                return Json(new { Success = true, ex = "" });
            }
            catch (Exception ex)
            {

                return Json(new { Success = false, ex = ex.Message.ToString() });
            }
        }

        // GET: /AllImages/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AllProductImages allproductimages = await db.AllProductImages.FindAsync(id);
            if (allproductimages == null)
            {
                return HttpNotFound();
            }
            return View(allproductimages);
        }

        // GET: /AllImages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /AllImages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,ImageName,ImagePath,UserID,CreatedDate,UpdatedDate,Sort,Description,Notes")] AllProductImages allproductimages)
        {
            if (ModelState.IsValid)
            {
                db.AllProductImages.Add(allproductimages);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(allproductimages);
        }

        // GET: /AllImages/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AllProductImages allproductimages = await db.AllProductImages.FindAsync(id);
            if (allproductimages == null)
            {
                return HttpNotFound();
            }
            return View(allproductimages);
        }

        // POST: /AllImages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ImageName,ImagePath,UserID,CreatedDate,UpdatedDate,Sort,Description,Notes")] AllProductImages allproductimages)
        {
            if (ModelState.IsValid)
            {
                db.Entry(allproductimages).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(allproductimages);
        }

        // GET: /AllImages/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AllProductImages allproductimages = await db.AllProductImages.FindAsync(id);
            if (allproductimages == null)
            {
                return HttpNotFound();
            }
            return View(allproductimages);
        }

        // POST: /AllImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            AllProductImages allproductimages = await db.AllProductImages.FindAsync(id);
            db.AllProductImages.Remove(allproductimages);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //public ActionResult DeleteImageID(int ImageIDs)
        //{
        //    try
        //    {

        //    }

        //    catch (Exception ex)
        //    {

        //    }
        //}


        public ActionResult DeleteAll(int[] ImageIDs)
        {
            try
            {


                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    foreach (int ID in ImageIDs)
                    {
                        AllProductImages obj = db.AllProductImages.Find(ID);
                        db.AllProductImages.Remove(obj);
                    }
                    db.SaveChanges();
                    return Json(new { Success = true, Ex = "" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Ex = ex.Message.ToString() });
            }
        }








        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
