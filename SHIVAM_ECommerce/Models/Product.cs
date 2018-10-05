using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.Models
{
    public class Product : BaseClass
    {
        #region Properties
        [Required(ErrorMessage = "Product Name is Required.")]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "Product Quantity is Required.")]



        public string ProductCode { get; set; }
        public string Ranking { get; set; }

        public string SKU { get; set; }

        public string IDSKU { get; set; }





        #endregion  Properties

        #region ForeignKey Properties
        public int SupplierID { get; set; }

        public int ManuFacturerID { get; set; }

        public int CateogryID { get; set; }
        public int UnitOfMeasuresId { get; set; }

        #endregion

        #region Navigational Properties
        [ForeignKey("SupplierID")]

        public virtual Supplier Supplier { get; set; }

        [ForeignKey("CateogryID")]
        public virtual Category Category { get; set; }

        [ForeignKey("ManuFacturerID")]
        public virtual Manufacturer ManuFacturer { get; set; }
        public virtual ICollection<ProductAttributesRelation> ProductAttributes { get; set; }
        public virtual ICollection<ProductImages> ProductImages { get; set; }

        public virtual ICollection<ProductAttributeWithQuantity> ProductAttributesWithQuantity { get; set; }


        [ForeignKey("UnitOfMeasuresId")]
        public virtual UnitOfMeasures UnitOfMeasure { get; set; }
        #endregion

    }

    public class ProductAttributes
    {
        public int Id { get; set; }

        [Required]
        public string AttributeName { get; set; }

        public string AttributeDescription { get; set; }

    }
    public class ProductAttributesRelation
    {
        public int Id { get; set; }
        public int SupplierID { get; set; }
        public int ProductAttributesId { get; set; }


        [ForeignKey("ProductAttributesId")]
        public virtual ProductAttributes ProductAttributes { get; set; }
        [ForeignKey("SupplierID")]
        public virtual Supplier Supplier { get; set; }
    }

    public class UnitOfMeasures
    {
        public int Id { get; set; }

        [Required]
        public string UnitOfMeasuresName { get; set; }


    }


    public class ProductAttributeWithQuantity
    {

        public int Id { get; set; }
        public int ProductId { get; set; }
        public bool IsFeatured { get; set; }
        public string AttributeValues { get; set; }

        public decimal? lowQuantityThreshold { get; set; }
        public decimal? highQuantityThreshold { get; set; }
        public int? StatusId { get; set; }
        public bool? IsAvailable { get; set; }
        public bool IsActive { get; set; }
        public decimal? Discount { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitWeight { get; set; }
        public decimal UnitInStock { get; set; }
        public decimal UnitsInOrder { get; set; }
        public int ProductQuantity { get; set; }

        [Required(ErrorMessage = "Product Price is Required.")]
        public decimal ProductPrice { get; set; }

        public decimal? Weight { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }


        [ForeignKey("StatusId")]
        public virtual ProductStatus status { get; set; }
    }



}