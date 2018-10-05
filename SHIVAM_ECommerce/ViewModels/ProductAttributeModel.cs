using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHIVAM_ECommerce.ViewModels
{
    public class ProductAttributeModel
    {

        public List<ProductAttributeModelInner> ColumnsData { get; set; }
        public int Quantity { get; set; }

        public decimal price { get; set; }

        public decimal weight { get; set; }
        public bool IsFeatured { get; set; }
        public decimal? lowQuantityThreshold { get; set; }
        public decimal? highQuantityThreshold { get; set; }
        public int? StatusId { get; set; }
        public List<ProductImagesViewModel> Images { get; set; }

        public int Id { get; set; }

    }

    public class ProductAttributeModelInner
    {
        public int AttributeID { get; set; }
        public string Value { get; set; }
      


    }

    
    public class ProductAttributeWithQuantityVM
    {
        public string AttributeValuesWithId { get; set; }
        public int ProductQuantity { get; set; }

    }
}
