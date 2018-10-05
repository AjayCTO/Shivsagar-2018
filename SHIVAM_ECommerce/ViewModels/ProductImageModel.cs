using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHIVAM_ECommerce.ViewModels
{
    public class ProductImagesViewModel
    {
        public int ImageID { get; set; }
        public string FileName { get; set; }

        public double Size { get; set; }
        public byte[] bytestring { get; set; }

        public int ID { get; set; }
    }

    public class ProductImagesAssignViewModel
    {
        public int ProductID { get; set; }

        public int ImageID { get; set; }
        public List<string> Path { get; set; }
    }
}
