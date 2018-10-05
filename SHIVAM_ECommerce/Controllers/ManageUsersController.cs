using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SHIVAM_ECommerce.Models;
using SHIVAM_ECommerce.Repository;
using SHIVAM_ECommerce.ViewModels;
using System.Linq.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SHIVAM_ECommerce.Extensions;
using System.Data.SqlClient;
using System.Configuration;
using SHIVAM_ECommerce.Attributes;
namespace SHIVAM_ECommerce.Controllers
{
    [CustomAuthorize]
    public class ManageUsersController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        private IRepository<Claims> _ClaimsRepository = null;
        private IRepository<ApplicationUser> _UsersRepository = null;
        private IRepository<IdentityUserClaim> _UserClaimsRepository = null;
        private UserManager<ApplicationUser> UserManager = null;

        public ManageUsersController()
        {
            this._ClaimsRepository = new Repository<Claims>();
            this._UsersRepository = new Repository<ApplicationUser>();
            this._UserClaimsRepository = new Repository<IdentityUserClaim>();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

        }



        //[Authorize(Roles = "superadmin,Supplier")]
        //
        // GET: /Manage/
        public ActionResult Index()
        {
            var _users = db.Users.ToList();
            var _user = db.Users.FirstOrDefault();
            return View(_users);
        }


        public ActionResult LoadData()
        {

            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var searchitem = Request["search[value]"];
            //Find Order Column
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;


            // dc.Configuration.LazyLoadingEnabled = false; // if your table is relational, contain foreign key
            var _userIDs = new List<string>();
            if (CurrentUserData.SupplierID != -1)
            {
                _userIDs = db.Suppliers.Where(x => x.Id == CurrentUserData.SupplierID).Select(x => x.UserID).ToList();
            }
            var v = CurrentUserData.SupplierID == -1 ? db.Users.AsQueryable() : db.Users.Where(x => _userIDs.Contains(x.Id)).AsQueryable();
            if (!string.IsNullOrEmpty(searchitem))
            {

                v = v.Where(b => b.UserName.ToLower().Contains(searchitem.ToLower()));
            }
            //SORT
            if (!(string.IsNullOrEmpty(sortColumn) || sortColumn == "" && string.IsNullOrEmpty(sortColumnDir)))
            {
                v = v.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = v.Count();
            var data = v.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data.Select(x => new { x.Id, x.UserName, x.UpdatedDate }) }, JsonRequestBehavior.AllowGet);
        }




        public ActionResult EditRoles(string Id)
        {
            var _AllRoles = db.Roles.ToList();


            var _user = _UsersRepository.GetById(Id);
            var _newuser = new UserRoleViewModel() { UserId = Id, UserName = _user.UserName, Roles = new List<CustomRoles>() };
            var _countRoles = _user.Roles.Count();
            foreach (var item in _AllRoles)
            {
                var _cRole = _countRoles > 0 ? _user.Roles.Where(x => x.Role.Name.ToLower() == item.Name.ToLower()).FirstOrDefault() : null;
                _newuser.Roles.Add(new CustomRoles() { Role = item.Name, Id = item.Id, IsChecked = _cRole == null ? false : true });
            }
            return View(_newuser);
        }


        [HttpPost]
        public ActionResult EditRoles(UserRoleViewModel model)
        {
            try
            {
                var _user = UserManager.FindById(model.UserId);

                var _userRoles = db.Roles.ToList();
                foreach (var item in model.Roles)
                {

                    var _ExistingRole = _user.Roles.Where(x => x.Role.Name.ToLower() == item.Role.ToLower()).FirstOrDefault();
                    if (_ExistingRole == null && item.IsChecked == true)
                    {
                        UserManager.AddToRole(_user.Id, item.Role);
                    }

                    if (_ExistingRole != null && item.IsChecked == false)
                    {
                        UserManager.RemoveFromRole(_user.Id, item.Role);

                    }


                }





                db.SaveChanges();
                this.AddNotification("Roles updated successfully.", NotificationType.SUCCESS);
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult Edit(string Id)
        {
            var _AllClaims = CurrentUserData.SupplierID == -1 ? _ClaimsRepository.GetAll().Where(x => x.Role == "SuperAdmin").ToList() : _ClaimsRepository.GetAll().Where(x => x.Role == "Supplier").ToList();
      
            //var _AllClaims = CurrentUserData.SupplierID == -1 ? _ClaimsRepository.GetAll() : _ClaimsRepository.GetAll().Where(x => x.Role == "Supplier").ToList();
            var _user = _UsersRepository.GetById(Id);
            var _newuser = new UserViewModel() { UserId = Id, UserName = _user.UserName, Claims = new List<CustomClaims>() };
            var _countClaims = _user.Claims.Count();
            foreach (var item in _AllClaims)
            {
                var _cClaim = _countClaims > 0 ? _user.Claims.Where(x => x.ClaimValue.ToLower() == item.ClaimValue.ToLower()).FirstOrDefault() : null;
                _newuser.Claims.Add(new CustomClaims() { ClaimValue = item.ClaimValue, ClaimType = item.ClaimType, Id = item.Id, IsChecked = _cClaim == null ? false : true });
            }
            return View(_newuser);
        }

        [HttpPost]
        public ActionResult Edit(UserViewModel model)
        {
            try
            {
                var _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString.ToString();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var _userClaims = _UserClaimsRepository.GetAll().Where(x => x.User.Id == model.UserId).ToList();
                    foreach (var item in model.Claims)
                    {
                        var _user = UserManager.FindById(model.UserId);

                        var _ExistingClaim = _userClaims.Where(x => x.ClaimValue.ToLower() == item.ClaimValue.ToLower()).FirstOrDefault();
                        if (_ExistingClaim == null && item.IsChecked == true)
                        {

                            String query = "INSERT INTO [dbo].[AspNetUserClaims]([ClaimType],[ClaimValue],[UserId],[ClaimID],[IsActive],[DisplayLabel],[Discriminator],[User_Id]) VALUES(@ClaimType,@ClaimValue,@UserId,@ClaimID,@IsActive,@DisplayLabel,@Discriminator,@User_Id)";
                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@ClaimType", item.ClaimType);
                                command.Parameters.AddWithValue("@ClaimValue", item.ClaimValue);
                                command.Parameters.AddWithValue("@UserId", model.UserId);
                                command.Parameters.AddWithValue("@ClaimID", item.Id);

                                command.Parameters.AddWithValue("@IsActive", "True");
                                command.Parameters.AddWithValue("@DisplayLabel", item.ClaimValue);
                                command.Parameters.AddWithValue("@Discriminator", "ApplicationUserClaim");
                                command.Parameters.AddWithValue("@User_Id", model.UserId);

                                int _result = command.ExecuteNonQuery();

                                if (_result < 0)
                                    Console.WriteLine("Error inserting data into Database!");
                            }

                            //var _newClaim = new System.Security.Claims.Claim(item.ClaimType, item.ClaimValue);
                            //UserManager.AddClaim(model.UserId, _newClaim);



                        }

                        if (_ExistingClaim != null && item.IsChecked == false)
                        {
                            var _newClaim = new System.Security.Claims.Claim(_ExistingClaim.ClaimType, _ExistingClaim.ClaimValue);
                            UserManager.RemoveClaim(model.UserId, _newClaim);

                        }
                    }


                }
                _UserClaimsRepository.Save();
                this.AddNotification("Claims updated successfully.", NotificationType.SUCCESS);
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}