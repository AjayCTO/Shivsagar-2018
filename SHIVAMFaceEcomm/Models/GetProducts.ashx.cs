using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace SHIVAMFaceEcomm.Models
{
    /// <summary>
    /// Summary description for GetProducts
    /// </summary>
    public class GetProducts : IHttpHandler
    {

        public List<object> newcolumns = null;
        private int filteredRows = 0;
        private int SupplierId = Convert.ToInt32(ConfigurationManager.AppSettings["SupplierID"]);
        public void ProcessRequest(HttpContext context)
        {
            int displayLength = int.Parse(context.Request["displayLength"]);
            int displayStart = int.Parse(context.Request["displayStart"]);
            //int sortCol = int.Parse(context.Request["iSortCol_0"]);
            //string sortDir = context.Request["sSortDir_0"];
            string search = context.Request["searchText"];
            string filterText = context.Request["filtertext"];
            string categories = context.Request["Categories"];
            string Lowprice = context.Request["lowprice"];
            string Highprice = context.Request["highprice"];
            string IsFeatured = context.Request["isFeatured"];
       
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var allProducts = new List<object[]>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("SP_GetAllSortedProductsFrontFace", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramDisplayLength = new SqlParameter()
                {
                    ParameterName = "@DisplayLength",
                    Value = displayLength
                };
                cmd.Parameters.Add(paramDisplayLength);

                SqlParameter paramDisplayStart = new SqlParameter()
                {
                    ParameterName = "@DisplayStart",
                    Value = displayStart
                };
                cmd.Parameters.Add(paramDisplayStart);

                SqlParameter paramSortCol = new SqlParameter()
                {
                    ParameterName = "@SortCol",
                    Value = "productName"
                };
                cmd.Parameters.Add(paramSortCol);

                SqlParameter paramSortDir = new SqlParameter()
                {
                    ParameterName = "@SortDir",
                    Value = "asc"
                };
                cmd.Parameters.Add(paramSortDir);

                SqlParameter paramSearchString = new SqlParameter()
                {
                    ParameterName = "@SearchText",
                    Value = string.IsNullOrEmpty(search) ? null : search
                };
                cmd.Parameters.Add(paramSearchString);
                  SqlParameter paramFilterText = new SqlParameter()
                {
                    ParameterName = "@FilterText",

                    Value = string.IsNullOrEmpty(filterText) ? "" : filterText
                    //"([Color] in ('Red','Green') OR [Size] in ('X','L')) AND"
                };
                cmd.Parameters.Add(paramFilterText);
                SqlParameter paramCategoriesText = new SqlParameter()
                {
                    ParameterName = "@Categories",

                    Value = string.IsNullOrEmpty(categories) ? "" : categories
                };
                cmd.Parameters.Add(paramCategoriesText);
                SqlParameter paramlowpriceText = new SqlParameter()
                {
                    ParameterName = "@LowPrice",

                    Value = string.IsNullOrEmpty(Lowprice) ? "" : Lowprice
                };
                cmd.Parameters.Add(paramlowpriceText);

                SqlParameter paramhighpriceText = new SqlParameter()
                {
                    ParameterName = "@HighPrice",

                    Value = string.IsNullOrEmpty(Highprice) ? "" : Highprice
                };
                cmd.Parameters.Add(paramhighpriceText);



                SqlParameter paramIsFeaturedText = new SqlParameter()
                {
                    ParameterName = "@IsFeatured",

                    Value = string.IsNullOrEmpty(IsFeatured) ? "0" : IsFeatured
                };
                cmd.Parameters.Add(paramIsFeaturedText);
               
               

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                using (rdr)
                {


                    allProducts = Read(rdr).ToList();

                    //TempData["cols"] = newcolumns;

                }
                //while (rdr.Read())
                //{
                //    filteredRows = Convert.ToInt32(rdr["TotalCount"]);
                //    Employee employee = new Employee();
                //    employee.Id = Convert.ToInt32(rdr["Id"]);
                //    employee.FirstName = rdr["FirstName"].ToString();
                //    employee.LastName = rdr["LastName"].ToString();
                //    employee.Gender = rdr["Gender"].ToString();
                //    employee.JobTitle = rdr["JobTitle"].ToString();
                //    employees.Add(employee);
                //}
            }

            var result = new
            {
                iTotalRecords = GetProductsTotalCount(),
                iTotalDisplayRecords = filteredRows,
                aaData = allProducts,
                aoColumns = newcolumns
            };

            JavaScriptSerializer js = new JavaScriptSerializer();
            context.Response.Write(js.Serialize(result));
        }
        private IEnumerable<object[]> Read(DbDataReader reader)
        {
            newcolumns = new List<object>();
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
                    filteredRows = Convert.ToInt32(reader["TotalCount"]);

                    values.Add(reader.GetValue(i));
                }
                k++;
                yield return values.ToArray();
            }
        }
        private int GetProductsTotalCount()
        {
            int totalEmployees = 0;
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("[GetProductsCount] 1", con);
                con.Open();
                totalEmployees = (int)cmd.ExecuteScalar();
            }
            return totalEmployees;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}