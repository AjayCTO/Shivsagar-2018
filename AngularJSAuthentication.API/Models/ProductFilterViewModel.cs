using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularJSAuthentication.API.Models
{
    public class ProductFilterViewModel
    {

        public int displayLength { get; set; }
        public int displayStart { get; set; }

        public string searchText { get; set; }

        public string filterText { get; set; }

        public string Categories { get; set; }

        public string lowprice { get; set; }
        public string highprice { get; set; }

        public string isFeatured { get; set; }

        public string isMostSale { get; set; }

        public string TopRated { get; set; }

    }
}