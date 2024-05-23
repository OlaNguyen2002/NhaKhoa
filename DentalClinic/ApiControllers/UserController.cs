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

namespace DentalClinic.ApiControllers
{
    public class UserController : ApiBaseController
    {
        // GET: User
        [HttpGet]
        public JsonResult GetListUser()
        {
            UserService userService = new UserService();
            List<User> list = userService.GetListUser();
            return Success(list);
        }
        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetInforUser()
        {
            try
            {
                User user = new User();
                if (Request.Headers.Authorization != null)
                {
                    string token = Request.Headers.Authorization.ToString();
                    UserService userService = new UserService();
                     user = userService.GetUserByToken(token);
                    if (user == null) return Unauthorized();
                    user.Password = "";
                }
                

                return Success(user);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpPost]
        [AllowAnonymous]
        public JsonResult Login(User model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserService userService = new UserService(connect);

                        if (string.IsNullOrEmpty(model.Account) || string.IsNullOrEmpty(model.Password)) throw new Exception("Email/Tên đăng nhập/Mật khẩu không được để trống.");

                        User userLogin = userService.GetUserByUserName(model.Account, transaction);
                        if (userLogin == null) throw new Exception("Người dùng này không tồn tại");

                        if (userLogin.Enable == false) return Error("Tài khoản này đã bị khóa.");

                        string password = SecurityProvider.EncodePassword(userLogin.UserId, model.Password);
                        if (!userLogin.Password.Equals(password)) throw new Exception("Mật khẩu không đúng.");

                        string deviceId = Guid.NewGuid().ToString().ToLower();
                        string token = SecurityProvider.CreateToken(userLogin.UserId, userLogin.Password, deviceId);

                        DateTime now = DateTime.Now;
                        UserToken userToken = userService.GetUserTokenByUserId(userLogin.UserId, transaction);
                        userToken.Token = token;
                        userToken.CreateTime = HelperProvider.GetSeconds(now);

                        if (!userService.UpdateUserToken(userToken, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        transaction.Commit();
                        return Success(new { token, deviceId });
                    }
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult Register(User model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserService userService = new UserService(connect);

                        if (string.IsNullOrEmpty(model.Name)) throw new Exception("Họ và tên người dùng không được để trống.");
                        if (string.IsNullOrEmpty(model.Account) || userService.CheckDuplicateUser(model.Account, transaction) != null) throw new Exception("Tên đăng nhập không được để trống.");
                        if (string.IsNullOrEmpty(model.Email) || userService.CheckDuplicateUser(model.Email, transaction) != null) throw new Exception("Email không được để trống.");
                        if (string.IsNullOrEmpty(model.Password)) throw new Exception("Password không được để trống.");

                        User user = new User();
                        user.UserId = Guid.NewGuid().ToString();
                        user.Name = model.Name;
                        user.Account = model.Account;
                        user.Email = model.Email;
                        user.Password = SecurityProvider.EncodePassword(user.UserId, model.Password);
                        user.Enable = true;
                        DateTime now = DateTime.Now;
                        user.CreateTime = HelperProvider.GetSeconds(now);

                        if (!userService.InsertNewUser(user, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        UserToken userToken = new UserToken();
                        userToken.UserTokenId = Guid.NewGuid().ToString();
                        userToken.UserId = user.UserId;
                        userToken.CreateTime = HelperProvider.GetSeconds(now);
                        if (!userService.InsertUserToken(userToken, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

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
        public JsonResult Logout()
        {
            try
            {
                string token = Request.Headers.Authorization.ToString();
                UserService userService = new UserService();
                userService.RemoveUserToken(token);
                return Success();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [ApiTokenRequire]
        public JsonResult ChangePassword(UserRequest userRequset)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        try
                        {
                            string token = Request.Headers.Authorization.ToString();

                            UserService userService = new UserService(connect);
                            User user = userService.GetUserByToken(token, transaction);
                            if (user == null) return Error("Người dùng không tồn tại");
                            string password = SecurityProvider.EncodePassword(user.UserId, userRequset.Password);
                            if (!user.Password.Equals(password)) return Error("Mật khẩu cũ không đúng");
                            else
                            {
                                userRequset.NewPassword = SecurityProvider.EncodePassword(user.UserId, userRequset.NewPassword);
                                if (!userService.UpdatePassword(user.UserId, userRequset.NewPassword, transaction)) return Error();

                                transaction.Commit();
                                return Success();
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Error(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Error();
            }
        }

        [HttpPost]
        [ApiTokenRequire]
        public JsonResult UpdateInfoUser(UserUpdateModel model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        string token = Request.Headers.Authorization.ToString();
                        UserService userService = new UserService(connect);
                        User user = userService.GetUserByToken(token, transaction);
                        if (user == null) return Unauthorized();

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

                        if (string.IsNullOrEmpty(user.Phone))
                        {
                            user.Phone = model.Phone.Trim();
                            if (userService.CheckPhoneExist(user.Phone, transaction)) return Error("Số điện thoại đã tồn tại.");
                        }
                        user.Address = model.Address;
                        if (!userService.UpdateInforUser(user, transaction)) return Error();
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
