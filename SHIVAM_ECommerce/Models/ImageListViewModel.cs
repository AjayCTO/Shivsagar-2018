using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHIVAM_ECommerce.Controllers
{
    public class ImageListViewModel
    {
        public int pageSize { get; set; }
        public int page { get; set; }
        public int ProductID { get; set; }
        public string SearchString { get; set; }
    }
}
