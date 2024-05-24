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
using System.Web.UI.WebControls;
using System.Web.UI;

namespace DentalClinic.Areas.Admin.ApiControllers
{
    public class ManageUserController : ApiAdminBaseController
    {
        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult GetListUser(string keyword, bool? enable, int? page)
        {
            try
            {
                UserAdminService userAdminService = new UserAdminService();
                ManageUserService manageUserService = new ManageUserService();
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();
                if (!page.HasValue) page = 1;
                return Success(manageUserService.GetListUser(page.Value, keyword, enable));
            }
            catch (Exception ex)
            {
                return Error();
            }
        }

        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult GetListUserNew()
        {
            try
            {
                UserAdminService userAdminService = new UserAdminService();
                ManageUserService manageUserService = new ManageUserService();
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();
                return Success(manageUserService.GetListUserNew());
            }
            catch (Exception ex)
            {
                return Error();
            }
        }
        
        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult GetUserDetail(string userId)
        {
            try
            {
                UserAdminService userAdminService = new UserAdminService();
                ManageUserService manageUserService = new ManageUserService();
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();
                return Success(manageUserService.GetUserById(userId));
            }
            catch (Exception ex)
            {
                return Error();
            }
        }
        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult ChangeUserActivation(string userId, bool enable)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserAdminService userAdminService = new UserAdminService(connect);
                        UserService userService = new UserService(connect);
                        ManageUserService manageUserService = new ManageUserService(connect);
                        UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                        if (userAdmin == null) return Unauthorized();
                        User user = userService.GetUserById(userId, transaction);
                        if (user == null) return Error();
                        user.Enable = enable;
                        if (!manageUserService.ChangeUserEnable(user, transaction)) return Error();

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
        [ApiAdminTokenRequire]
        public JsonResult UpdateUser(User model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserAdminService userAdminService = new UserAdminService(connect);
                        ManageUserService manageUserService = new ManageUserService(connect);
                        UserService userService = new UserService(connect);

                        UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                        if (userAdmin == null) return Unauthorized();
                        User user = userService.GetUserById(model.UserId, transaction);
                        if (user == null) return Error();
                        user.Name = model.Name.Trim();
                        user.SearchName = model.SearchName.Trim();
                        if (!string.IsNullOrEmpty(model.Avatar))
                        {
                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            var path = System.Web.HttpContext.Current.Server.MapPath(Constant.AVATAR_USER_PATH + filename);
                            HelperProvider.Base64ToImage(model.Avatar, path);
                            if (!HelperProvider.DeleteFile(user.Avatar)) return Error();
                            user.Avatar = Constant.AVATAR_USER_URL + filename;
                        }


                        user.Email = model.Email.Trim();
                        if (manageUserService.CheckAccountEmail(user.Email, user.UserId, transaction)) return Error("Email đã tồn tại.");

                        if (!string.IsNullOrEmpty(model.Password))
                        {
                            user.Password = SecurityProvider.EncodePassword(user.UserId, model.Password);
                        }
                        user.Address = model.Address;
                        user.Gender = model.Gender;
                        user.Phone = model.Phone;
                        user.CreateTime = HelperProvider.GetSeconds();
                        if (!manageUserService.UpdateUser(user, transaction)) return Error();


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
    }
}
