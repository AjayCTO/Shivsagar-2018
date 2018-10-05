using SHIVAM_ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SHIVAM_ECommerce.Functions
{
    public static class ExportToExcel
    {

        public static FileStreamResult getExcelData(string _query, string FileName)
        {

            string NewconnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var _path = HttpContext.Current.Server.MapPath("~/DownloadedFiles/" + FileName);
            //StreamWriter CsvfileWriter = new StreamWriter(@"D:\testfile.csv");
            StreamWriter CsvfileWriter = new StreamWriter(new MemoryStream(), Encoding.UTF8);
            string sqlselectQuery = _query;
            SqlCommand sqlcmd = new SqlCommand();

            SqlConnection spContentConn = new SqlConnection(NewconnectionString);
            sqlcmd.Connection = spContentConn;
            sqlcmd.CommandTimeout = 0;
            sqlcmd.CommandText = sqlselectQuery;
            spContentConn.Open();
            using (spContentConn)
            {
                using (SqlDataReader sdr = sqlcmd.ExecuteReader())
                // using (CsvfileWriter)
                {
                    //This Block of code for getting the Table Headers
                    DataTable Tablecolumns = new DataTable();
                    var _data = "";
                    for (int i = 0; i < sdr.FieldCount; i++)
                    {
                        Tablecolumns.Columns.Add(sdr.GetName(i));
                    }
                    CsvfileWriter.WriteLine(string.Join(",", Tablecolumns.Columns.Cast<DataColumn>().Select(csvfile => csvfile.ColumnName)));
                    //This block of code for getting the Table Headers
                    var _count = Tablecolumns.Columns.Count;
                    while (sdr.Read())
                    {
                        _data = "";
                        for (int i = 0; i < _count; i++)
                        {
                            _data += sdr[i].ToString() + ",";
                        }

                        CsvfileWriter.WriteLine(_data);
                    }
                    //based on your Table columns you can increase and decrese columns

                }
            }



            CsvfileWriter.Flush();
            CsvfileWriter.BaseStream.Seek(0, SeekOrigin.Begin);


            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("content-disposition", String.Format("attachment; filename={0}", FileName + ".csv"));
            return new FileStreamResult(CsvfileWriter.BaseStream, "text/csv");
        }


        public static List<string> GetCustomColumnNames(int SupplierID)
        {
            var _db = new ApplicationDbContext();

            var _AttributeSelection = _db.ProductAttributesRelation.Where(x => x.SupplierID == SupplierID).Select(x => x.ProductAttributes.AttributeName).ToList();
            _AttributeSelection.Add("Quantity"); _AttributeSelection.Add("Cost");

            return _AttributeSelection;
        }

        public static List<ProductAttributesRelation> GetCustomColumns(int SupplierID)
        {
            var _db = new ApplicationDbContext();

            var _AttributeSelection = _db.ProductAttributesRelation.Where(x => x.SupplierID == SupplierID).ToList();

            return _AttributeSelection;
        }
        public static FileStreamResult getSampleFile(int SupplierID)
        {
            string FileName = "Sheet1";
            string NewconnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var _path = HttpContext.Current.Server.MapPath("~/DownloadedFiles/" + FileName);
            StreamWriter CsvfileWriter = new StreamWriter(new MemoryStream(), Encoding.UTF8);

            //This Block of code for getting the Table Headers
            DataTable Tablecolumns = new DataTable();
            var _FieldList = new List<string>();

            var _AttributeSelection = GetCustomColumnNames(SupplierID);
            _FieldList.Add("Name"); _FieldList.Add("ProductCode"); _FieldList.Add("Description"); _FieldList.Add("categoryName"); _FieldList.Add("UnitOfMeasure");
            _FieldList.Add("ManuFacturer"); _FieldList.Add("HighQuantityThreshold"); _FieldList.Add("LowQuantityThreshold"); _FieldList.Add("IsFeatured");
            _FieldList.Add("ProductStatus");
            for (int i = 0; i < _FieldList.Count(); i++)
            {
                Tablecolumns.Columns.Add(_FieldList[i]);
            }

            for (int i = 0; i < _AttributeSelection.Count(); i++)
            {
                Tablecolumns.Columns.Add(_AttributeSelection[i]);
            }

            CsvfileWriter.WriteLine(string.Join(",", Tablecolumns.Columns.Cast<DataColumn>().Select(csvfile => csvfile.ColumnName)));

            CsvfileWriter.Flush();
            CsvfileWriter.BaseStream.Seek(0, SeekOrigin.Begin);


            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("content-disposition", String.Format("attachment; filename={0}", FileName + ".xls"));
            return new FileStreamResult(CsvfileWriter.BaseStream, "application/vnd.ms-excel");
            //return new FileStreamResult(CsvfileWriter.BaseStream, "text/csv");
            //return new FileStreamResult(CsvfileWriter.BaseStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        }
    }
}