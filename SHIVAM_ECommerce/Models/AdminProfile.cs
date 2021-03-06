﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.Models
{
    public class AdminProfile : BaseClass
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Logo { get; set; }
    }
}