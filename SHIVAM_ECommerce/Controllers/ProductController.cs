
using SHIVAM_ECommerce.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SHIVAM_ECommerce.Models;
using SHIVAM_ECommerce.Repository;
using SHIVAM_ECommerce.Functions;
using System.Data.OleDb;
using System.Data;
using LinqToExcel;
using System.Data.Common;
using System.Data.Entity.Validation;
using System.Configuration;
using System.Data.SqlClient;
using SHIVAM_ECommerce.Attributes;
using Syncfusion.XlsIO;
using SHIVAM_ECommerce.Extensions;
using System.Net;
namespace SHIVAM_ECommerce.Controllers
{
    [CustomAuthorize]
    public class ProductController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IRepository<Product> _repository = null;
        private IRepository<ProductImages> _Imagesrepository = null;
        private IRepository<ProductAttributeWithQuantity> _Attributerepository = null;
        public List<object> newcolumns = null;
        public List<string> _columns = new List<string>();
        public int SupplierId = 0;
        public ProductController()
        {
            this._repository = new Repository<Product>();
            this._Imagesrepository = new Repository<ProductImages>();
            this._Attributerepository = new Repository<ProductAttributeWithQuantity>();
            this.newcolumns = new List<object>();



        }
        // GET: Product
        public ActionResult Index()
        {
            return RedirectToAction("GetAllProducts");
        }


        public FileStreamResult DownloadSample()
        {
            return ExportToExcel.getSampleFile(CurrentUserData.SupplierID);

        }

        public FileStreamResult DownloadCategory()
        {
            return ExportToExcel.getCategory(CurrentUserData.SupplierID);
        }
        public ActionResult ImportExport()
        {
            return View();
        }

        public FileStreamResult ExportData()
        {
            return ExportToExcel.getExcelData("select * from suppliers", "Supplier");

        }

        private IEnumerable<object[]> Read(DbDataReader reader)
        {
            var k = 0;
            while (reader.Read())
            {

                var values = new List<object>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (k == 0)
                    {
                        newcolumns.Add(reader.GetName(i));
                    }


                    values.Add(reader.GetValue(i));
                }
                k++;
                yield return values.ToArray();
            }
        }

        public ActionResult GetAllProducts()
        {
            _columns = ExportToExcel.GetCustomColumnNames(SupplierId);

            if (Session["Drpsupplierid"] != null)
            {
                SupplierId = (int)Session["Drpsupplierid"];
            }
            else {
                SupplierId = CurrentUserData.SupplierID;
            }
           
            //SupplierId = 1005;
            var ctx = new ApplicationDbContext();
            var _supplierdata = ctx.Suppliers.FirstOrDefault(x => x.Id == 1006);
            SupplierId = SupplierId == -1 ? (_supplierdata != null ? _supplierdata.Id : -1) : SupplierId;
            ViewBag.SupplierID = SupplierId;
            ViewBag.Suppliers = CurrentUserData.SupplierID == -1 ? db.Suppliers.ToList() : new List<Supplier>();
           var _productAttributes = db.ProductAttributesRelation.Where(x => x.SupplierID == CurrentUserData.SupplierID).ToList();
            //var _productAttributes = db.ProductAttributesRelation.Where(x => x.SupplierID == _supplierdata.Id).ToList();

            using (var cmd = ctx.Database.Connection.CreateCommand())
            {
                ctx.Database.Connection.Open();
                cmd.CommandText = "EXEC SP_GetAllSortedProducts 5,0,'productName','ASC',NULL, " + SupplierId + "";
               // cmd.CommandText = "EXEC SP_GetAllSortedProducts 5,0,'productName','ASC',NULL, " + _supplierdata.Id + "";

                var _xdta = cmd.ExecuteReader();
                if (_xdta != null)
                {

                    using (var reader = _xdta)
                    {
                        var model = Read(reader).ToList();
                        TempData["cols"] = newcolumns;
                        if (CurrentUserData.SupplierID != -1 && _productAttributes.Count() == 0)
                        {
                            this.AddNotification("Please add product attribute before adding product.", NotificationType.WARNING);
                            return RedirectToAction("AddAttributesForSupplier", "ProductAttributes");
                        }
                        else
                        {
                            return View(model);

                        }
                    }
                }
            }
            return View(new List<object>());
        }


        public ActionResult GetColumnNames(int SupplierID)
        {
            try
            {

                Session["Drpsupplierid"] = SupplierID;
                SupplierID = CurrentUserData.IsSuperAdmin == true ? SupplierID : CurrentUserData.SupplierID;
                var ctx = new ApplicationDbContext();
                var _List = new List<object>();
                using (var cmd = ctx.Database.Connection.CreateCommand())
                {
                    ctx.Database.Connection.Open();
                    cmd.CommandText = "EXEC SP_GetAllSortedProducts 5,0,'productName','ASC',NULL, " + SupplierID + "";
                    using (var reader = cmd.ExecuteReader())
                    {
                        var model = Read(reader).ToList();
                        _List = newcolumns;                       
                        return Json(new { Success = true, Data = _List, ex = "" }, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            catch (Exception ex)
            {

                return Json(new { Success = true, Data = "", ex = ex.Message.ToString(), JsonRequestBehavior.AllowGet });
            }
        }
        private string GetAttributeValuesForSql(Row a, List<ProductAttributesRelation> Columns)
        {
            var _string = "";


            foreach (var _columnData in Columns)
            {

                _string += _columnData.ProductAttributesId.ToString() + "@@" + a[_columnData.ProductAttributes.AttributeName] + "##";

            }


            return _string;
        }
        private string GetAttributeValuesForSqlSync(IRange a, List<ProductAttributesRelation> Columns, IRange _headerRow)
        {
            var _string = "";


            foreach (var _columnData in Columns)
            {

                _string += _columnData.ProductAttributesId.ToString() + "@@" + a.Cells[GetColumnIndex(_headerRow, _columnData.ProductAttributes.AttributeName)].Value.ToString() + "##";

            }


            return _string;
        }
        [HttpPost]
        public JsonResult UploadExcel()
        {

            try
            {

                SupplierId = CurrentUserData.SupplierID;
                string _ErrorString = "";
                var FileUpload = Request.Files["FileUpload"];
                List<string> data = new List<string>();
                if (FileUpload != null)
                {
                    if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {


                        string filename = FileUpload.FileName;
                        string targetpath = Server.MapPath("~/ProductImages/");
                        FileUpload.SaveAs(targetpath + filename);
                        string pathToExcelFile = targetpath + filename;
                        var connectionString = "";


                        ExcelEngine excelEngine = new ExcelEngine();
                        IApplication application = excelEngine.Excel;

                        string fileName = Guid.NewGuid().ToString() + ".xlsx";
                        IWorkbook workbook = application.Workbooks.Create(1);
                        workbook.Version = ExcelVersion.Excel2007;
                        workbook.DetectDateTimeInValue = true;


                        workbook = excelEngine.Excel.Workbooks.Open(pathToExcelFile, ExcelOpenType.Automatic, ExcelParseOptions.Default);
                        IWorksheet worksheet = workbook.Worksheets[0];
                        var _colRow = worksheet.Rows[0];
                        var _ColCount = _colRow.Cells.Count();
                        var _headerRow = worksheet.Rows[0];

                        var _count = 0;
                        var _RowCount = worksheet.Rows.Count();
                        for (int i = 1; i < _RowCount; i++)
                        {
                            var _Row = worksheet.Rows[i];


                            if (!_Row.IsBlank)
                            {


                                var _validityChecker = IsValidObjectSync(_Row, _count + 1, _headerRow);
                                if (!_validityChecker.IsValid)
                                {

                                    _ErrorString = _ErrorString + "<br/>" + _validityChecker.ErrorString;

                                }

                                _count = _count + 1;

                            }
                        }



                        if (_ErrorString.Length > 0)
                        {
                            return Json(new { Success = (_ErrorString.Length > 0) ? false : true, ErrorString = _ErrorString }, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            var _Customcolumns = ExportToExcel.GetCustomColumns(SupplierId);
                            var NewconnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                            using (SqlConnection connection = new SqlConnection(NewconnectionString))
                            {
                                for (int i = 1; i < _RowCount; i++)
                                {
                                    var a = worksheet.Rows[i];


                                    if (!a.IsBlank)
                                    {

                                        var Name = a.Cells[GetColumnIndex(_headerRow, "Name")].Value.ToString().Replace("'", "''");
                                        var productCode = a.Cells[GetColumnIndex(_headerRow, "ProductCode")].Value.ToString().Replace("'", "''");
                                        var description = a.Cells[GetColumnIndex(_headerRow, "Description")].Value.ToString().Replace("'", "''");
                                        var categoryName = a.Cells[GetColumnIndex(_headerRow, "categoryName")].Value.ToString().Replace("'", "''");
                                        var UnitOfMeasure = a.Cells[GetColumnIndex(_headerRow, "UnitOfMeasure")].Value.ToString().Replace("'", "''");
                                        var manufacturer = a.Cells[GetColumnIndex(_headerRow, "ManuFacturer")].Value.ToString().Replace("'", "''");

                                        string _Query = "DECLARE @return_value int,@ProductID int;";
                                        _Query += "SELECT	@ProductID = -1;";
                                        _Query += "EXEC	@return_value = [dbo].[InsertProduct] @Name='" + Name + "',@ProductCode = '" + productCode + "',@Description = '" + description + "',@categoryName = '" + categoryName + "',@UnitOfMeasure = '" + UnitOfMeasure + "',@ManuFacturer = '" + manufacturer + "',@SupplierID = '" + SupplierId + "',@ProductID = @ProductID OUTPUT;";
                                        _Query += "SELECT	@ProductID as N'ProductID'";
                                        SqlCommand command = new SqlCommand(_Query, connection);
                                        command.CommandTimeout = 640;
                                        command.Connection.Open();
                                        var _productID = command.ExecuteScalar();

                                        _Query = "EXEC [dbo].[InsertProductAttributes] @AttributeValues='" + GetAttributeValuesForSqlSync(a, _Customcolumns, _headerRow) + "', @ProductPrice = '" + a.Cells[GetColumnIndex(_headerRow, "Cost")].Value.ToString() + "', @ProductQuantity = '" + a.Cells[GetColumnIndex(_headerRow, "Quantity")].Value.ToString() + "', @ProductId = '" + _productID.ToString() + "', @UnitInStock = '" + a.Cells[GetColumnIndex(_headerRow, "Quantity")].Value.ToString() + "', @UnitWeight = '0',@HighQuantityThreshold='" + a.Cells[GetColumnIndex(_headerRow, "HighQuantityThreshold")].Value.ToString() + "',@LowQuantityThreshold='" + a.Cells[GetColumnIndex(_headerRow, "LowQuantityThreshold")].Value.ToString() + "',@ProductStatus='" + a.Cells[GetColumnIndex(_headerRow, "ProductStatus")].Value.ToString() + "';";

                                        command = new SqlCommand(_Query, connection);
                                        command.ExecuteNonQuery();

                                        command.CommandTimeout = 640;
                                        command.Connection.Close();
                                    }
                                }

                            }

                        }
                        //deleting excel file from folder  
                        if ((System.IO.File.Exists(pathToExcelFile)))
                        {
                            System.IO.File.Delete(pathToExcelFile);
                        }

                        return Json(new { Success = (_ErrorString.Length > 0) ? false : true, ErrorString = _ErrorString }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        _ErrorString = "Only Excel file format is allowed";

                        return Json(new { Success = (_ErrorString.Length > 0) ? false : true, ErrorString = _ErrorString }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    _ErrorString = "Please Choose Excel File";

                    return Json(new { Success = (_ErrorString.Length > 0) ? false : true, ErrorString = _ErrorString }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                return Json(new { Success = false, ErrorString = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }


        private ImportValid IsValidObject(Row a, int _Index)
        {
            var _ValidObj = new ImportValid();
            _ValidObj.IsValid = true;
            _ValidObj.ErrorString = "";
            try
            {


                if (string.IsNullOrEmpty(a["Name"].ToString())) _ValidObj.ErrorString += "<br/> Product name is Required";
                if (string.IsNullOrEmpty(a["categoryName"].ToString())) _ValidObj.ErrorString += "<br/> category name is Required";
                if (string.IsNullOrEmpty(a["UnitOfMeasure"].ToString())) _ValidObj.ErrorString += "<br/> UnitofMasure is Required";
                if (string.IsNullOrEmpty(a["ManuFacturer"].ToString())) _ValidObj.ErrorString += "<br/> Manufacturer name is Required";
                if (string.IsNullOrEmpty(a["HighQuantityThreshold"].ToString())) _ValidObj.ErrorString += "<br/> High Quantity Threshold is Required";
                if (string.IsNullOrEmpty(a["LowQuantityThreshold"].ToString())) _ValidObj.ErrorString += "<br/>  Low Quantity Threshold is Required";
                if (string.IsNullOrEmpty(a["ProductStatus"].ToString())) _ValidObj.ErrorString += "<br/> Product Status is Required";

                foreach (var item in _columns.ToList())
                {
                    var _column = item.ToString();
                    decimal _checkDecimal = -1;
                    if (string.IsNullOrEmpty(a[_column].Value.ToString())) _ValidObj.ErrorString += "<br/> " + _column + " is Required";

                    if (!string.IsNullOrEmpty(a[_column].Value.ToString()) && (_column.ToLower() == "quantity" || _column.ToLower() == "cost" || _column.ToLower() == "HighQuantityThreshold" || _column.ToLower() == "LowQuantityThreshold"))
                    {
                        if (!decimal.TryParse(a[_column].Value.ToString(), out _checkDecimal)) _ValidObj.ErrorString += "<br/> " + _column + " is not a valid number";
                    }
                }

                if (_ValidObj.ErrorString.Length > 0)
                {
                    _ValidObj.IsValid = false;
                    _ValidObj.ErrorString = "<b>Row " + _Index.ToString() + "</b>" + _ValidObj.ErrorString;

                }
                return _ValidObj;
            }
            catch (Exception)
            {

                throw;
            }
        }


        private int GetColumnIndex(IRange row, string _Column)
        {
            int count = 0;
            foreach (var item in row.Cells)
            {
                if (row.Cells[count].Value.ToString() == _Column)
                {
                    return count;
                }
                count = count + 1;
            }

            return count;

        }
        private ImportValid IsValidObjectSync(IRange a, int _Index, IRange _headerRow)
        {
            var _ValidObj = new ImportValid();
            _ValidObj.IsValid = true;
            _ValidObj.ErrorString = "";
            var productNames = db.Products.Where(x => x.SupplierID == CurrentUserData.SupplierID).Select(x => x.ProductName).ToList();


            //var valueTest = a.Cells[GetColumnIndex(_headerRow, "Name")].Value.ToString();

            //var valueTest = a.Cells[0].Value.ToString();

            if (productNames.Any(s => s.Contains(a.Cells[GetColumnIndex(_headerRow, "Name")].Value.ToString())))
            {
                var ProductName = a.Cells[GetColumnIndex(_headerRow, "Name")].Value.ToString();
                _ValidObj.ErrorString += "<br/> Product with name = " + ProductName + " already exists please update the quantity from product details page.";
            }     
            try
            {


                if (string.IsNullOrEmpty(a.Cells[GetColumnIndex(_headerRow, "Name")].Value.ToString())) _ValidObj.ErrorString += "<br/> Product name is Required";
                if (string.IsNullOrEmpty(a.Cells[GetColumnIndex(_headerRow, "categoryName")].Value.ToString())) _ValidObj.ErrorString += "<br/> category name is Required";
                if (string.IsNullOrEmpty(a.Cells[GetColumnIndex(_headerRow, "UnitOfMeasure")].Value.ToString())) _ValidObj.ErrorString += "<br/> UnitofMasure is Required";
                if (string.IsNullOrEmpty(a.Cells[GetColumnIndex(_headerRow, "ManuFacturer")].Value.ToString())) _ValidObj.ErrorString += "<br/> Manufacturer name is Required";
                if (string.IsNullOrEmpty(a.Cells[GetColumnIndex(_headerRow, "HighQuantityThreshold")].Value.ToString())) _ValidObj.ErrorString += "<br/> High Quantity Threshold is Required";
                if (string.IsNullOrEmpty(a.Cells[GetColumnIndex(_headerRow, "LowQuantityThreshold")].Value.ToString())) _ValidObj.ErrorString += "<br/>  Low Quantity Threshold is Required";
                if (string.IsNullOrEmpty(a.Cells[GetColumnIndex(_headerRow, "ProductStatus")].Value.ToString())) _ValidObj.ErrorString += "<br/> Product Status is Required";

                foreach (var item in _columns.ToList())
                {
                    var _column = item.ToString();
                    decimal _checkDecimal = -1;
                    if (string.IsNullOrEmpty(a.Cells[GetColumnIndex(_headerRow, _column)].Value.ToString())) _ValidObj.ErrorString += "<br/> " + _column + " is Required";

                    if (!string.IsNullOrEmpty(a.Cells[GetColumnIndex(_headerRow, _column)].Value.ToString()) && (_column.ToLower() == "quantity" || _column.ToLower() == "cost" || _column.ToLower() == "HighQuantityThreshold" || _column.ToLower() == "LowQuantityThreshold"))
                    {
                        if (!decimal.TryParse(a.Cells[GetColumnIndex(_headerRow, _column)].Value.ToString(), out _checkDecimal)) _ValidObj.ErrorString += "<br/> " + _column + " is not a valid number";
                    }
                }

                if (_ValidObj.ErrorString.Length > 0)
                {
                    _ValidObj.IsValid = false;
                    var TempIndex = _Index + 1;
                    _ValidObj.ErrorString = "<b>Row " + TempIndex.ToString() + "</b>" + _ValidObj.ErrorString;

                }
                return _ValidObj;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult AssignProductImages(int ProductID)
        {
            ViewBag.ProductID = ProductID;
            var _db = new ApplicationDbContext();
            var _Data = _db.ProductImages.Where(x => x.ProductQuantityId == ProductID).ToList();
            ViewBag.AlreadySelected = _Data.Select(x => x.ImageName).ToList();
           var Imgdata= _Data.Select(p => new {Id=p.Id ,ImageName=p.ImageName,ImagePath=p.ImageName,ProductQuantityid=p.ProductQuantityId}).ToList();
           ViewBag.ImageData = Imgdata;
            return View();
        }


        public ActionResult Create()
        {
            return View(new ProductViewmodel());
        }

        public void SaveProductDetail(Product _Product)
        {
            var _listAttributes = new List<ProductAttributeModelInner>();
            var _productAttributes = db.ProductAttributesRelation.Where(x => x.SupplierID == CurrentUserData.SupplierID).ToList();

            foreach (var _item in _productAttributes)
            {
                var _productAttr = new ProductAttributeModelInner();
                _productAttr.AttributeID = _item.ProductAttributesId;
                _productAttr.Value = " ";
                _listAttributes.Add(_productAttr);
            }

            var _productRel = new ProductAttributeWithQuantity();
            _productRel.IsAvailable = true;
            _productRel.AttributeValues = GetAttributeValues(_listAttributes);
            _productRel.ProductPrice = 0;
            _productRel.ProductQuantity = 1;
            _productRel.ProductId = _Product.Id;
            _productRel.UnitInStock = 1;
            _productRel.Weight = 0;
            _productRel.UnitWeight = 0;
            _productRel.IsFeatured = false;
            _productRel.lowQuantityThreshold = 1;
            _productRel.highQuantityThreshold = 10;
            _productRel.IsActive = true;

            _Attributerepository.Insert(_productRel);

            _Attributerepository.Save();
        }

        [HttpPost]

        public ActionResult Create(ProductViewmodel Model)
        {

            try
            {
                var product = db.Products.FirstOrDefault(x => x.ProductName.ToLower() == Model.ProductName.ToLower() && x.SupplierID == CurrentUserData.SupplierID);

                if (product == null)
                {
                    #region Product Save
                    var _product = new Product();
                    _product.ManuFacturerID = Model.ManufacturerID;
                    _product.ProductCode = Model.ProductCode;
                    _product.ProductName = Model.ProductName;
                    _product.CateogryID = Model.CategoryID;
                    _product.SupplierID = CurrentUserData.SupplierID;
                    _product.ManuFacturerID = Model.ManufacturerID;
                    _product.SKU = Model.SKU;
                    _product.CreatedDate = DateTime.Now;
                    _product.UpdatedDate = DateTime.Now;
                    _product.UnitOfMeasuresId = Model.UnitOfMeasureID;
                    _product.Description = Model.Description;
                    _repository.Insert(_product);
                    _repository.Save();
                    SaveProductDetail(_product);
                    #endregion


                    return Json(new { Success = true, ex = "", data = "", id = _product.Id });
                }
                else
                {
                    return Json(new { Success = false, ex = "Product alreay added please update the quantity.", data = "", id = 0 });
                }
            }
            catch (Exception ex)
            {

                return Json(new { Success = false, ex = ex.Message.ToString(), data = "", id = 0 });

            }


        }
        //public ActionResult Create(ProductViewmodel Model)
        //{

        //    try
        //    {
        //        #region Product Save
        //        var _product = new Product();
        //        _product.ManuFacturerID = Model.ManufacturerID;
        //        _product.ProductCode = Model.ProductCode;
        //        _product.ProductName = Model.ProductName;
        //        _product.CateogryID = Model.CategoryID;
        //        _product.SupplierID = CurrentUserData.SupplierID;
        //        _product.ManuFacturerID = Model.ManufacturerID;
        //        _product.SKU = Model.SKU;
        //        _product.CreatedDate = DateTime.Now;
        //        _product.UpdatedDate = DateTime.Now;
        //        _product.UnitOfMeasuresId = Model.UnitOfMeasureID;
        //        _product.Description = Model.Description;
        //        _repository.Insert(_product);
        //        _repository.Save();
        //        SaveProductDetail(_product);
        //        #endregion


        //        return Json(new { Success = true, ex = "", data = "", id = _product.Id });
        //    }
        //    catch (Exception ex)
        //    {

        //        return Json(new { Success = false, ex = ex.Message.ToString(), data = "", id = 0 });

        //    }


        //}
      [HttpPost]
        public ActionResult Deletes(int productID)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var product = db.Products.FirstOrDefault(x => x.Id == productID);
                    db.Products.Remove(product);
                    db.SaveChanges();
                    return Json(new { Success = true, ex = "", data = "" });
                }
                catch (Exception ex)
                {

                    return Json(new { Success = false, ex = ex.Message.ToString(), data = "" });

                }

            }

            return Json(new { Success = true, ex = "", data = "" });
            //return View(GetProduct(productID));
        }

        [HttpPost]
        public ActionResult Delete(ProductViewmodel Model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var product = db.Products.FirstOrDefault(x => x.Id == Model.ProductID);
                    db.Products.Remove(product);
                    return RedirectToAction("GetAllProducts");
                }
                catch (Exception ex)
                {

                    return Json(new { Success = false, ex = ex.Message.ToString(), data = "" });

                }

            }

            return Json(new { Success = true, ex = "", data = "" });
        }

        public ActionResult Edit(int productID)
        {
            return View(GetProduct(productID));
        }

        [HttpPost]
        public ActionResult Edit(ProductViewmodel Model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var supplierId = -1;
                    if (Session["Drpsupplierid"] != null)
                    {
                        supplierId = (int)Session["Drpsupplierid"];
                    }
                    else
                    {
                        supplierId = CurrentUserData.SupplierID;
                    }
                    
                    #region Product Save
                    var _product = _repository.GetById(Model.ProductID);
                    _product.ManuFacturerID = Model.ManufacturerID;
                    _product.ProductCode = Model.ProductCode;
                    _product.ProductName = Model.ProductName;
                    _product.CateogryID = Model.CategoryID;
                    _product.SupplierID = supplierId;
                    _product.ManuFacturerID = Model.ManufacturerID;
                    _product.SKU = Model.SKU;
                    _product.UpdatedDate = DateTime.Now;
                    _product.UnitOfMeasuresId = Model.UnitOfMeasureID;
                    _product.Description = Model.Description;
                    _repository.Save();
                    #endregion


                    return Json(new { Success = true, ex = "", data = "", id = _product.Id });
                }
                catch (Exception ex)
                {

                    return Json(new { Success = false, ex = ex.Message.ToString(), data = "" });

                }

            }

            return Json(new { Success = true, ex = "", data = "" });
        }
        public ProductViewmodel GetProduct(int productID)
        {
            var _ProductViewModel = new ProductViewmodel();
            var _Product = _repository.GetById(productID);
            _ProductViewModel.ProductID = _Product.Id;
            _ProductViewModel.SupplierID = _Product.SupplierID;
            _ProductViewModel.UnitOfMeasureID = _Product.UnitOfMeasuresId;
            _ProductViewModel.ManufacturerID = _Product.ManuFacturerID;
            _ProductViewModel.SKU = _Product.SKU;
            _ProductViewModel.CategoryID = _Product.CateogryID;
            _ProductViewModel.ProductName = _Product.ProductName;
            _ProductViewModel.ProductCode = _Product.ProductCode;
            if (_Product.Description == null)
            {
                _ProductViewModel.Description = _Product.Description;
            }
            else
            {
               var  a = _Product.Description.Replace("\"", "");
        
                _ProductViewModel.Description = a.Replace("\n", "<br>");
        
            }
            _ProductViewModel.IDSKU = _Product.IDSKU;

            return _ProductViewModel;
        }
        public ActionResult ProductDetails(int productID)
        {
            var productstauts = db.ProductStatus.ToList();
            ViewBag.productstatuslist = productstauts;

            var _ProductViewModel = GetProduct(productID);
            var _ProductAttributes = _Attributerepository.GetAll().Where(x => x.ProductId == productID && x.IsActive == true).ToList();
            _ProductViewModel.allAttributes = new List<ProductAttributeModel>();
            foreach (var _item in _ProductAttributes)
            {
                var _newitem = new ProductAttributeModel();
                _newitem.price = _item.ProductPrice;
                _newitem.weight = _item.UnitWeight;
                _newitem.Quantity = _item.ProductQuantity;
                _newitem.highQuantityThreshold = _item.highQuantityThreshold;
                _newitem.lowQuantityThreshold = _item.lowQuantityThreshold;
                _newitem.IsFeatured = _item.IsFeatured;
                _newitem.StatusId = _item.StatusId;
                _newitem.ColumnsData = new List<ProductAttributeModelInner>();
                _newitem.ColumnsData = GetColumnsDataSplitted(_item.AttributeValues);
                _newitem.Images = new List<ProductImagesViewModel>();
                _newitem.Images = GetImages(_item.Id);
                _newitem.Id = _item.Id;
                _ProductViewModel.allAttributes.Add(_newitem);
            }

            return View(_ProductViewModel);
        }

        private List<ProductImagesViewModel> GetImages(int ProductID)
        {
            var _listImages = _Imagesrepository.GetProductImages(ProductID);
            var _productImages = new List<ProductImagesViewModel>();
            var _count = 0;
            var _Path = "";
            foreach (var _item in _listImages)
            {
                _count += 1;
                var _Newitem = new ProductImagesViewModel();
                _Newitem.ImageID = _count;
                _Newitem.FileName = _item.ImagePath;
                _Path = System.Web.Hosting.HostingEnvironment.MapPath("~/ProductImages/") + _item.ImagePath;
                _Newitem.bytestring = System.IO.File.ReadAllBytes(_Path);
                _Newitem.ID = _item.Id;
                _productImages.Add(_Newitem);

            }

            return _productImages;
        }


        /// <summary>
    
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
// product details page
        public ActionResult Details(int ProductID)
        {
            var Productdetailsid = db.ProductAttributeWithQuantity.Where(x => x.Id == ProductID).Select(x => x.ProductId).FirstOrDefault();
            int Productid = Productdetailsid;

            var _ProductViewModel = new ProductViewmodel();
            var _Product = _repository.GetById(Productid);
            _ProductViewModel.ProductID = _Product.Id;
            _ProductViewModel.SupplierID = _Product.SupplierID;
            _ProductViewModel.UnitOfMeasureID = _Product.UnitOfMeasuresId;
            _ProductViewModel.ManufacturerID = _Product.ManuFacturerID;
            _ProductViewModel.SKU = _Product.SKU;
            _ProductViewModel.CategoryID = _Product.CateogryID;
            _ProductViewModel.ProductName = _Product.ProductName;
            _ProductViewModel.ProductCode = _Product.ProductCode;
            if (_Product.Description == null)
            {
                _ProductViewModel.Description = _Product.Description;
            }
            else
            {
                var a = _Product.Description.Replace("\"", "");

                _ProductViewModel.Description = a.Replace("\n", "<br>");

            }
            _ProductViewModel.IDSKU = _Product.IDSKU;
            var _ProductAttributes = _Attributerepository.GetAll().Where(x => x.Id == ProductID && x.IsActive == true).ToList();

            _ProductViewModel.allAttributes = new List<ProductAttributeModel>();
            foreach (var _item in _ProductAttributes)
            {
                var _newitem = new ProductAttributeModel();
                _newitem.price = _item.ProductPrice;
                _newitem.weight = _item.UnitWeight;
                _newitem.Quantity = _item.ProductQuantity;
                _newitem.highQuantityThreshold = _item.highQuantityThreshold;
                _newitem.lowQuantityThreshold = _item.lowQuantityThreshold;
                _newitem.IsFeatured = _item.IsFeatured;
                _newitem.StatusId = _item.StatusId;
                _newitem.ColumnsData = new List<ProductAttributeModelInner>();
                _newitem.ColumnsData = GetColumnsDataSplitted(_item.AttributeValues);
                _newitem.Images = new List<ProductImagesViewModel>();
                _newitem.Images = GetImages(_item.Id);
                _newitem.Id = _item.Id;
                _ProductViewModel.allAttributes.Add(_newitem);
            }
            return View(_ProductViewModel);
        }



        [HttpGet]
        public ActionResult GetDataImages(int ProductID)
        {
            try
            {

                var _db = new ApplicationDbContext();
                var _ProductQuantityIds = _db.ProductAttributeWithQuantity.Where(x => x.Id == ProductID).Select(x => x.Id);
                var _listImages = _db.ProductImages.Where(x => _ProductQuantityIds.Contains(x.ProductQuantityId)).ToList();
                var _productImages = new List<ProductImagesViewModel>();
                var _count = 0;
                foreach (var _item in _listImages)
                {
                    _count += 1;
                    var _Newitem = new ProductImagesViewModel();
                    _Newitem.ImageID = _count;
                    _Newitem.FileName = _item.ImagePath;
                    _productImages.Add(_Newitem);

                }
                return Json(new { Success = true, ex = "", data = _productImages }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, ex = ex.Message.ToString(), data = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<ProductAttributeModelInner> GetColumnsDataSplitted(string p)
        {
            string[] parts1 = p.Split(new string[] { "##" }, StringSplitOptions.RemoveEmptyEntries);
            var _ReturnModel = new List<ProductAttributeModelInner>();
            foreach (var _item in parts1.ToList())
            {
                string[] parts2 = _item.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                var _attributeID = parts2[0];


                var _value = "";
                if (parts2.Length > 1)
                {
                    _value = parts2[1] == null ? "N/A" : parts2[1];
                }

                
                _ReturnModel.Add(new ProductAttributeModelInner { AttributeID = Convert.ToInt16(_attributeID), Value = _value });
            }

            return _ReturnModel;
        }

        [HttpPost]
        public ActionResult ProductDetails(ProductViewmodel Model)
        {

            try
            {

                // DeleteOldAttributes(Model.ProductID);
                var _ProductAttributes = _Attributerepository.GetAll().Where(x => x.ProductId == Model.ProductID).ToList();

                var _productRel = new ProductAttributeWithQuantity();
                #region Product Attributes
                Model.allAttributes = (Model.allAttributes == null ? new List<ProductAttributeModel>() : Model.allAttributes);
                if (Model.allAttributes.Count() > 0)
                {
                    foreach (var _item in Model.allAttributes)
                    {
                        var _tempProductRel = _ProductAttributes != null && _ProductAttributes.Count() > 0 ? _ProductAttributes.Where(x => x.Id == _item.Id).FirstOrDefault() : new ProductAttributeWithQuantity();
                        _tempProductRel = _tempProductRel == null ? new ProductAttributeWithQuantity() : _tempProductRel;
                        _productRel = _tempProductRel;
                        _productRel.IsAvailable = true;
                        _productRel.AttributeValues = GetAttributeValues(_item.ColumnsData);
                        _productRel.ProductPrice = _item.price;
                        _productRel.ProductQuantity = _item.Quantity;
                        _productRel.ProductId = Model.ProductID;
                        _productRel.UnitInStock = _item.Quantity;
                        _productRel.Weight = _item.weight;
                        _productRel.UnitWeight = _item.weight;
                        _productRel.StatusId = _item.StatusId;
                        _productRel.IsFeatured = _item.IsFeatured;
                        _productRel.lowQuantityThreshold = _item.lowQuantityThreshold;
                        _productRel.highQuantityThreshold = _item.highQuantityThreshold;
                        _productRel.IsActive = true;
                        if (_productRel.Id == 0)
                        {
                            _Attributerepository.Insert(_productRel);

                        }
                        _Attributerepository.Save();

                        #region Product Image
                        _item.Images = (_item.Images == null ? new List<ProductImagesViewModel>() : _item.Images);
                        if (_item.Images.Count() > 0)
                        {
                            foreach (var _image in _item.Images)
                            {
                                if (_image.ID == 0)
                                {

                                    byte[] bitmap = _image.bytestring;
                                    var _imageName = "";
                                    if (bitmap != null)
                                    {
                                        _imageName = Guid.NewGuid().ToString() + ".png";

                                        var _Path = System.Web.Hosting.HostingEnvironment.MapPath("~/ProductImages/");
                                        using (Image image = Image.FromStream(new MemoryStream(bitmap)))
                                        {
                                            var _CompletePath = _Path + _imageName;
                                            image.Save(_CompletePath, ImageFormat.Png);  // Or Png
                                        }
                                    }

                                    var _productImage = new ProductImages();
                                    _productImage.ImageName = _image.FileName;
                                    _productImage.ImagePath = _imageName;
                                    _productImage.ProductQuantityId = _productRel.Id;
                                    //_productImage.ProductId = Model.ProductID;
                                    _productImage.CreatedDate = DateTime.Now;
                                    _productImage.UpdatedDate = DateTime.Now;
                                    _Imagesrepository.Insert(_productImage);
                                    _Imagesrepository.Save();
                                }
                            }
                        }
                        #endregion
                    }
                }
                #endregion



                return Json(new { Success = true, ex = "", data = "" });
            }
            catch (Exception ex)
            {

                return Json(new { Success = false, ex = ex.Message.ToString(), data = "" });

            }

        }
        public FileStreamResult ExportProducts()
        {
            return ExportToExcel.getExcelData("EXEC SP_GetAllSortedProducts 8000,0,'productName','ASC',NULL, " + CurrentUserData.SupplierID + "", "AllProducts");

        }

        [HttpPost]

        public ActionResult DeleteProductDetail(int ID)
        {
            try
            {
                var _orderItems = db.OrderItems.Where(x => x.ProductID == ID).Count();
                if (_orderItems <= 0)
                {

                    var _obj = db.ProductAttributeWithQuantity.Where(x => x.Id == ID).FirstOrDefault();
                    _obj.IsActive = false;
                    db.SaveChanges();
                    return Json(new { Success = true, ex = "", data = "" });

                }
                return Json(new { Success = false, ex = "This item is currently in use", data = "" });

            }
            catch (Exception ex)
            {

                return Json(new { Success = false, ex = ex.Message.ToString(), data = "" });

            }
        }

        [HttpPost]

        public ActionResult DeleteProductImage(int ID)
        {
            try
            {

                _Imagesrepository.Delete(ID);
                _Imagesrepository.Save();

                return Json(new { Success = true, ex = "", data = "" });
            }
            catch (Exception ex)
            {

                return Json(new { Success = false, ex = ex.Message.ToString(), data = "" });

            }
        }
        public void DeleteOldAttributes(int productID)
        {
            var _db = new ApplicationDbContext();
            var _PrdouctData = _db.ProductAttributeWithQuantity.Where(x => x.ProductId == productID);
            if (_PrdouctData.Count() > 0)
            { _db.ProductAttributeWithQuantity.RemoveRange(_PrdouctData); _db.SaveChanges(); }
        }
        private string GetAttributeValues(List<ProductAttributeModelInner> list)
        {
            var _string = "";
            if (list.Count() > 0)
            {
                foreach (var _item in list)
                {
                    _string += _item.AttributeID.ToString() + "@@" + _item.Value + "##";
                }

            }
            return _string;
        }
    }
}