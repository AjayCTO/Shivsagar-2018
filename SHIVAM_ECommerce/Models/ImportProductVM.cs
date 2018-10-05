using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.Models
{
    public class ImportProductVM
    {
        public string ProductName { get; set; }

        public string Manufacturer { get; set; }

        public string SupplierId { get; set; }

        public string UnitofMasure { get; set; }

        public string SKU { get; set; }

        public string ProductCode { get; set; }

        public string category { get; set; }

        public string Description { get; set; }

        

        public List<ImportProductVMAtrribute> Attributes { get; set; }
    }
    public class ImportProductVMAtrribute
    {
        public string AtrributeName { get; set; }

        public string AtributeValue { get; set; }

        public int Quantity { get; set; }

        public decimal Cost { get; set; }



    }


    public class ImportValid
    {
        public bool IsValid { get; set; }

        public string ErrorString { get; set; }
    }
}