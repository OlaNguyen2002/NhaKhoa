using DentalClinic.Filters;
using DentalClinic.Models;
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
    public class NotificationController : ApiBaseController
    {
        [HttpGet]
        [ApiTokenRequire]
        public JsonResult GetListNotification(int page)
        {
            try
            {
                UserService userService = new UserService();
                string token = Request.Headers.Authorization.ToString();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();
                NotificationService notificationService = new NotificationService();
                if (page <= 0) page = 1;
                return Success(notificationService.GetListNotification(user.UserId, page));

            }
            catch (Exception ex)
            {
                return Error();
            }
        }

        [HttpGet]
        [ApiTokenRequire]
        public JsonResult GetListNewNotification()
        {
            try
            {
                UserService userService = new UserService();
                string token = Request.Headers.Authorization.ToString();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();
                NotificationService notificationService = new NotificationService();
                return Success(notificationService.GetListNewNotification(user.UserId));

            }
            catch (Exception ex)
            {
                return Error();
            }
        }

        [HttpGet]
        [ApiTokenRequire]
        public JsonResult UpdateNotificationRead(long id)
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
                        NotificationService notificationService = new NotificationService(connect);
                        Models.Notification notification = notificationService.GetNotificationById(id, transaction);
                        notification.IsRead = true;
                        if (!notificationService.UpdateNotificationRead(notification, transaction)) return Error();
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

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetNumberNotificationForHomePage()
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserService userService = new UserService();
                        string token = Request.Headers.Authorization.ToString();
                        User user = userService.GetUserByToken(token);
                        if (user == null) return Unauthorized();

                        NotificationService notificationService = new NotificationService(connect);
                        int number = notificationService.GetNumberNotificationForHomePage(user.UserId, transaction);
                        return Success(new { numbernotification = number });
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
