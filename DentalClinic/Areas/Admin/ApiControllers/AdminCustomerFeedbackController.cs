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
    public class AdminCustomerFeedbackController : ApiAdminBaseController
    {
        [HttpGet]
        public JsonResult GetListCustomerFeedback()
        {
            try
            {
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();
                CustomerFeedbackService customerFeedbackService = new CustomerFeedbackService();
                return Success(customerFeedbackService.GetListCustomerFeedback());
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpPost]
        public JsonResult InsertFeedback(CustomerFeedback model)
        {
            try
            {
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();
                CustomerFeedbackService customerFeedbackService = new CustomerFeedbackService();
                CustomerFeedback customerFeedback = new CustomerFeedback();
                customerFeedback.CustomerName = model.CustomerName;
                customerFeedback.Contents = model.Contents;
                if (!string.IsNullOrEmpty(model.Avatar))
                {
                    string filename = Guid.NewGuid().ToString() + ".jpg";
                    var path = System.Web.HttpContext.Current.Server.MapPath(Constant.AVATAR_USER_PATH + filename);
                    HelperProvider.Base64ToImage(model.Avatar, path);
                    customerFeedback.Avatar = Constant.AVATAR_USER_URL + filename;
                }
                if (!customerFeedbackService.InsertFeedback(customerFeedback)) return Error();
                return Success("Tạo đánh giá thành công");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult DeleteFeedback(int CustomerFeedbackId)
        {
            try
            {
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();
                CustomerFeedbackService customerFeedbackService = new CustomerFeedbackService();
                CustomerFeedback customerFeedback = customerFeedbackService.GetFeedbackById(CustomerFeedbackId);

                //xoá Thumbnail
                if (!HelperProvider.DeleteFile(customerFeedback.Avatar)) return Error();

                if (!customerFeedbackService.DeleteFeedback(CustomerFeedbackId)) return Error();
                return Success("Xoá phản hồi thành công");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpPost]
        public JsonResult UpdateFeedback(CustomerFeedback model)
        {
            try
            {
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();
                CustomerFeedbackService customerFeedbackService = new CustomerFeedbackService();
                CustomerFeedback customerFeedback = customerFeedbackService.GetFeedbackById(model.CustomerFeedbackId);
                if (customerFeedback == null) throw new Exception("Không tìm thấy thông phản hồi");
                customerFeedback.CustomerName = model.CustomerName;
                customerFeedback.Contents = model.Contents;
                if (!string.IsNullOrEmpty(model.Avatar))
                {
                    //xoá file cũ
                    if (!HelperProvider.DeleteFile(customerFeedback.Avatar)) return Error();
                    //tạo file mới
                    string filename = Guid.NewGuid().ToString() + ".jpg";
                    var path = System.Web.HttpContext.Current.Server.MapPath(Constant.AVATAR_USER_PATH + filename);
                    HelperProvider.Base64ToImage(model.Avatar, path);
                    customerFeedback.Avatar = Constant.AVATAR_USER_URL + filename;
                }
                if (!customerFeedbackService.UpdateFeedback(customerFeedback)) return Error();
                return Success();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetFeedbackById(int CustomerFeedbackId)
        {
            try
            {
                CustomerFeedbackService customerFeedbackService = new CustomerFeedbackService();
                CustomerFeedback customerFeedback = customerFeedbackService.GetFeedbackById(CustomerFeedbackId);
                return Success(customerFeedback);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
