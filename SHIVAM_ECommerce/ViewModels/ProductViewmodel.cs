using SHIVAM_ECommerce.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.ViewModels
{
    public class ProductViewmodel
    {

        [Required(ErrorMessage = "Product Name is Required")]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Product Code")]
        public string ProductCode { get; set; }

        
        [Display(Name = "Product Price")]
        public decimal ProductPrice { get; set; }
        public int ProductID { get; set; }

        //[Required(ErrorMessage = "Product Quantity is Required")]
        //[Display(Name = "Product Quantity")]
        //public int ProductQuantity { get; set; }


        [Required(ErrorMessage = "Product SKU is Required")]
        [Display(Name = "Product SKU")]
        public string SKU { get; set; }

        [Required(ErrorMessage = "Product UnitOfMeasure is Required")]
        [Display(Name = "Product UnitOfMeasure")]
        public int UnitOfMeasureID { get; set; }


        
        
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "Product Category is Required")]
        [Display(Name = "Product Category")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Product Manufacturer is Required")]
        [Display(Name = "Product Manufacturer")]
        public int ManufacturerID { get; set; }


        //[Required(ErrorMessage = "Product UnitOfMeasure is Required")]
        //[Display(Name = "Product UnitOfMeasure")]
        //public UnitOfMeasures UnitOfMeasure { get; set; }

     

        //[Required(ErrorMessage = "Product Supplier is Required")]
        //[Display(Name = "Product Supplier")]
        //public Supplier Supplier { get; set; }

        //[Required(ErrorMessage = "Product Category is Required")]
        //[Display(Name = "Product Category")]
        //public Category Category { get; set; }

        //[Required(ErrorMessage = "Product Manufacturer is Required")]
        //[Display(Name = "Product Manufacturer")]
        //public Manufacturer Manufacturer { get; set; }
        public string IDSKU { get; set; }

        public string Description { get; set; }
        //public List<ProductImagesViewModel> Images { get; set; }
        public List<ProductAttributeModel> allAttributes { get; set; }

        public List<ProductAttributeWithQuantityVM> allAttributesValueWithQuantity { get; set; }
    }
}