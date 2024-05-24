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
    public class AdminSystemController : ApiAdminBaseController
    {
        [HttpPost]
        [AllowAnonymous]
        public JsonResult Login(UserLoginPost model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Account)) return Error("Tài khoản không được để trống.");
                if (string.IsNullOrEmpty(model.Password)) return Error("Mật khẩu không được để trống.");
                UserAdminService userAdminService = new UserAdminService();
                UserAdmin userAdmin = userAdminService.GetUserAdminByAccount(model.Account);
                if (userAdmin == null) return Error("Sai tài khoản hoặc mật khẩu.");
                string password = SecurityProvider.EncodePassword(userAdmin.UserAdminId, model.Password);
              //  if (userAdmin.Password != password) return Error("Sai tài khoản hoặc mật khẩu");
                string token = SecurityProvider.CreateToken(userAdmin.UserAdminId, userAdmin.Password, userAdmin.Account);
                if (!userAdminService.UpdateUserAdminToken(userAdmin.UserAdminId, token)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                return Success(token);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpGet]
        [APIAdminToken]
        public JsonResult LogOut()
        {
            try
            {
                UserAdminService userAdminService = new UserAdminService();
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();
                if (!userAdminService.UpdateUserAdminToken(userAdmin.UserAdminId, "")) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                return Success();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpGet]
        [APIAdminToken]
        public JsonResult GetUserAdminInfo()
        {
            try
            {
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();
                return Success(new { userAdmin.Name, userAdmin.Role,userAdmin.Account });
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
