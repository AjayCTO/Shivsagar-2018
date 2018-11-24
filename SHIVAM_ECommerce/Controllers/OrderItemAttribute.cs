using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHIVAM_ECommerce.Controllers
{
    class OrderItemAttribute
    {
        public int Id { get; set; }
        public string  AttributeName { get; set; }
        public string AttributeValue { get; set; }

    }
}
