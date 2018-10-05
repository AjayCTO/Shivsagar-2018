using SHIVAMFaceEcomm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
//using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Razorpay.Api;
using Microsoft.AspNet.Identity.EntityFramework;
using SHIVAMFaceEcomm.ViewModels;

namespace SHIVAMFaceEcomm.Controllers
{

    public class ShoppingCartController : Controller
    {
        SHIVAMECommerceDBNewEntities context = new SHIVAMECommerceDBNewEntities();
        ApplicationDbContext con = new ApplicationDbContext();
        //public ShoppingCartController()
        //    : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        //{
        //}

        //public ShoppingCartController(UserManager<ApplicationUser> userManager)
        //{
        //    UserManager = userManager;
        //}
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ShoppingCartController()
        {
        }

        public ShoppingCartController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // public UserManager<ApplicationUser> UserManager { get; private set; }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }
        public ActionResult Index()
        {
            var CustomerInfolist = new SHIVAMFaceEcomm.Models.Customer();
            var Cust_Info = new CustomerInfoViewModel();
            if (User.Identity.IsAuthenticated)
            {


                var Userid = User.Identity.GetUserId();
                CustomerInfolist = context.Customers.Where(x => x.UserID == Userid).FirstOrDefault();
                Cust_Info.FirstName = CustomerInfolist.FirstName;
                Cust_Info.LastName = CustomerInfolist.LastName;
                Cust_Info.Email = CustomerInfolist.Email;
                Cust_Info.Phone = CustomerInfolist.Phone;

            }

            ViewBag.Customer = Cust_Info;
            //MakeOnlinePayment();
            return View();
        }

        public string MakeOnlinePayment()
        {

            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("amount", 100); // this amount should be same as transaction amount
            input.Add("currency", "INR");
            input.Add("receipt", "12121");
            input.Add("payment_capture", 1);

            string key = "rzp_test_JHgzrt4lQIR3KO";
            string secret = "DZVn7UHC4F4Xbzawro0l76qx";

            RazorpayClient client = new RazorpayClient(key, secret);

            Razorpay.Api.Order order = client.Order.Create(input);

            var orderId = order["id"].ToString();



            return "2";
        }

        [HttpPost]
        public async Task<ActionResult> CompletePurchaseAndCreateSessionForUser(ShoppingCart CartDetails)
        {
            //add your stuff here to add into database

            try
            {
                var _controller = new AccountController();
                SHIVAMFaceEcomm.Models.Customer newCustomer = new SHIVAMFaceEcomm.Models.Customer();
                var _GlobaluserID = "";
                //Add Customer as User
                var user = new ApplicationUser() { UserName = CartDetails.CustomerData.userName, Email = CartDetails.CustomerData.email };
                var store = new UserStore<ApplicationUser>(con);
                if (!User.Identity.IsAuthenticated)
                {


                    var manager = new UserManager<ApplicationUser>(store);
                    IdentityResult result = manager.Create(user, CartDetails.CustomerData.password);
                    if (result.Succeeded)
                    {
                        newCustomer.UserID = user.Id;
                        _GlobaluserID = user.Id;
                        // context.SaveChanges();
                        UserManager.AddToRole(user.Id, "Customer");
                        // return RedirectToAction("Index");
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }

                else
                {

                    _GlobaluserID = User.Identity.GetUserId();
                    newCustomer = context.Customers.Where(x => x.UserID == _GlobaluserID).FirstOrDefault();

                }
                //Create Customer details

                newCustomer.CardExpMo = CartDetails.CustomerData.CardExpMo;
                newCustomer.CardExpYr = CartDetails.CustomerData.CardExpYr;
                newCustomer.CreditCard = CartDetails.CustomerData.CreditCard;
                newCustomer.CreditCardType = CartDetails.CustomerData.cardType.ToString();
                newCustomer.Email = CartDetails.CustomerData.email;
                newCustomer.FirstName = CartDetails.CustomerData.firstName;
                newCustomer.LastName = CartDetails.CustomerData.lastName;
                newCustomer.Phone = CartDetails.CustomerData.phone;

                newCustomer.CreatedDate = DateTime.Now;
                newCustomer.UpdatedDate = DateTime.Now;
                newCustomer.Sort = 33;

                if (!User.Identity.IsAuthenticated)
                {
                    context.Customers.Add(newCustomer);
                }
                context.SaveChanges();
                //Add Customer Address
                CustomerAddress address = new CustomerAddress();
                address.Address1 = CartDetails.CustAddress.address1;
                address.Address2 = CartDetails.CustAddress.address2;
                address.AddressType = CartDetails.CustAddress.Type;
                address.City = CartDetails.CustAddress.city;
                address.Country = CartDetails.CustAddress.country;
                address.CreatedDate = DateTime.Now;
                address.Description = "test";
                address.Region = CartDetails.CustAddress.region;
                address.State = CartDetails.CustAddress.state;
                address.UpdatedDate = DateTime.Now;
                context.CustomerAddresses.Add(address);
                context.SaveChanges();
                //Add Order and Items details
                //Add Order
                SHIVAMFaceEcomm.Models.Order order = new SHIVAMFaceEcomm.Models.Order();
                order.CreatedDate = DateTime.Now;
                order.CustomerID = newCustomer.Id;
                order.Description = "Order palced by " + newCustomer.FirstName;
                order.Isdeleted = false;
                order.IsPaid = false;
                order.OrderDate = DateTime.Now;
                order.OrderNumber = GenerateOrderNumber();
                order.RequiredDate = DateTime.Now;
                order.ShipDate = DateTime.Now.AddDays(7);
                order.TotalDiscount = 0;
                order.OrderStatusID = 2;
                order.TransactionStatus = "1";//Should be status of order
                order.UpdatedDate = DateTime.Now;
                context.Orders.Add(order);

                context.SaveChanges();
                decimal itemsTotal = 0;
                var itemsTotalQuanity = 0;

                CartDetails.CartItems.ForEach(p =>
                {
                    try
                    {
                        OrderItem orderitem = new OrderItem();
                        orderitem.CreatedDate = DateTime.Now;
                        orderitem.Discount = p.discount;
                        orderitem.Orders_Id = order.Id;
                        orderitem.ProductID = p.ProductId;
                        orderitem.Quantity = p.Quantity;
                        orderitem.TotalPrice = p.Cost;
                        orderitem.SupplierID = p.SupplierID;
                        orderitem.UnitPrice = p.Cost / p.Quantity;
                        orderitem.UpdatedDate = DateTime.Now;
                        context.OrderItems.Add(orderitem);
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {


                    }
                    itemsTotal = Convert.ToDecimal(itemsTotal) + p.Cost;
                    itemsTotalQuanity = itemsTotalQuanity + p.Quantity;
                });


                //Add order details
                //OrderDetail orderdetail = new OrderDetail();
                //orderdetail.BillDate = DateTime.Now;
                //orderdetail.CreatedDate = DateTime.Now;
                //orderdetail.Discount = false;
                //orderdetail.OrderID = order.Id;
                //orderdetail.OrderNumber = order.OrderNumber;
                //orderdetail.Total = itemsTotal;
                //orderdetail.Quantity = itemsTotalQuanity;
                //orderdetail.ShipDate = DateTime.Now.AddDays(2);
                //orderdetail.UpdatedDate = DateTime.Now;
                //context.OrderDetails.Add(orderdetail);
                //context.SaveChanges();
                //Send user email with all infor invoice
                //create user as customer and make him login
                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                
                
                //var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("SHIVAMFaceEcomm");
                //UserManager.UserTokenProvider = new Microsoft.AspNet.Identity.Owin.DataProtectorTokenProvider<ApplicationUser>(provider.Create("EmailConfirmation"));
                //var token = await UserManager.GenerateEmailConfirmationTokenAsync(_GlobaluserID);
                //string code = await UserManager.GenerateEmailConfirmationTokenAsync(_GlobaluserID);

                //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = _GlobaluserID, code = code }, protocol: Request.Url.Scheme);
                //await UserManager.SendEmailAsync(_GlobaluserID, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                if (!User.Identity.IsAuthenticated)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                // WebSecurity.Login(CartDetails.CustomerData.userName, CartDetails.CustomerData.password);\
                TempData["orderId"] = order.Id;
                TempData["CartItems"] = CartDetails.CartItems;
                //return Json("ok");
                return Json(new { Success = true, ex = "", stacktrace = "" });
            }
            catch (Exception e)
            {
                var st = new System.Diagnostics.StackTrace(e, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();

                return Json(new { Success = false, ex = e.InnerException.Message.ToString(), stacktrace = "Line no.:" + line + "Stack Trace::" + e.ToString() });
                //ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
            }

            return View("Index",CartDetails);
        }


        public ActionResult OrderConfirmation()
        {
            var orderId = TempData["orderId"];
            var CartItems = TempData["CartItems"] as IEnumerable<CartItems>;

            return View(CartItems);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        public ActionResult ConfirmOrderandProceed()
        {
            return View();
        }

        public string GenerateOrderNumber()
        {
            long ticks = DateTime.Now.Ticks;
            byte[] bytes = BitConverter.GetBytes(ticks);
            string id = Convert.ToBase64String(bytes)
                                    .Replace('+', '_')
                                    .Replace('/', '-')
                                    .TrimEnd('=');

            return id;
        }
    }
}
