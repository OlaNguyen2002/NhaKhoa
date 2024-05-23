using DentalClinic.Filters;
using DentalClinic.Models;
using DentalClinic.Providers;
using DentalClinic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace DentalClinic.Areas.Admin.ApiControllers
{
//    [Permission(Permissions = "ADMIN")]
    public class ManageUserAdminController : ApiAdminBaseController
    {
        [HttpGet]
        [APIAdminToken]
        public JsonResult GetListUserAdmin()
        {
            try
            {
                UserAdminService userAdminService = new UserAdminService();
                return Success(userAdminService.GetListUserAdmin());
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpGet]
        [APIAdminToken]
        public JsonResult GetUserAdmin(string userAdminId)
        {
            try
            {
                UserAdminService userAdminService = new UserAdminService();
                UserAdmin admin = userAdminService.GetUserAdminById(userAdminId);
                admin.Password = "";
                return Success(new { admin.Name, admin.Account, admin.Password, admin.UserAdminId, admin.CreateTime, admin.Role });
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpPost]
        [APIAdminToken]
        [AllowAnonymous]
        public JsonResult CreateUserAdmin(UserAdmin model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserAdminService userAdminService = new UserAdminService(connect);
                        model.UserAdminId = Guid.NewGuid().ToString();
                        model.Password = SecurityProvider.EncodePassword(model.UserAdminId, model.Password);
                        model.CreateTime = HelperProvider.GetSeconds();
                        if (!userAdminService.InsertUserAdmin(model, transaction)) return Error();
                        transaction.Commit();
                        return Success();
                    }
                }
            }
            catch (Exception ex)
            {
                return Error();
            }
        }

        [HttpPost]
        [APIAdminToken]
        public JsonResult UpdateUserAdmin(UserAdmin model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserAdminService userAdminService = new UserAdminService(connect);
                        UserAdmin admin = userAdminService.GetUserAdminById(model.UserAdminId, transaction);
                        if (admin == null) return Error("Tài khoản quản trị này không tồn tại.");
                        admin.Account = model.Account;
                        admin.Name = model.Name;
                        admin.Role = model.Role;
                        if (!string.IsNullOrEmpty(model.Password))
                        {
                            admin.Password = SecurityProvider.EncodePassword(admin.UserAdminId, model.Password);
                        }
                        if (!userAdminService.UpdateUserAdmin(admin, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
                        transaction.Commit();
                        return Success();
                    }
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpGet]
        [APIAdminToken]
        public JsonResult DeleteUserAdmin(string adminId)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserAdminService userAdminService = new UserAdminService(connect);
                        UserAdmin admin = userAdminService.GetUserAdminById(adminId, transaction);
                        if (admin == null) return Error("Tài khoản quản trị này không tồn tại.");
                        if (!userAdminService.DeleteUserAdmin(adminId, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
                        transaction.Commit();
                        return Success();
                    }
                }
            }
            catch (Exception ex)
            {
                return Error();
            }
        }
    }
}
