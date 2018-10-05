﻿ 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SHIVAMFaceEcomm.Models
{

    
    public enum CardType : byte
    {
        CreditCart,
        DebitCart,
        Paypal,
        Cashond
    }
    public class ShoppingCart
    {
        public List<CartItems> CartItems { get; set; }
        public Customerd CustomerData { get; set; }
        public CustomerAdd CustAddress { get; set; }
    }

    public class Customerd
    {
        public string firstName { get; set; }

        public string lastName { get; set; }
        public string phone { get; set; }

        public string email { get; set; }

        public string userName { get; set; }
        public string password { get; set; }

        public CardType cardType { get; set; }

        public string CreditCard { get; set; }
        public string CardExpMo { get; set; }
        public string CardExpYr { get; set; }
    }
    public class CustomerAdd
    {
        public string address1 { get; set; }

        public string address2 { get; set; }
        public string city { get; set; }

        public string state { get; set; }

        public string country { get; set; }
        public string region { get; set; }
        public string Type { get; set; }
    }

    public class CartItems
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public Decimal Cost { get; set; }

        public decimal discount { get; set; }

        public int SupplierID { get; set; }
    }


    public class ReviewModel
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public String Review { get; set; }
         
    }

    public class APIResponse
    {
        public int ID { get; set; }
        public bool Success { get; set; }

        public string Ex { get; set; }
    }
}