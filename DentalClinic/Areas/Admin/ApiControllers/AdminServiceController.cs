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
    public class AdminServiceController : ApiAdminBaseController
    {
        [HttpGet]
        public JsonResult GetListService(int? page, string keyword = "")
        {
            try
            {
                AdminServiceService adminService = new AdminServiceService();
                if (!page.HasValue) page = 1;
                return Success(adminService.GetListService(page.Value, keyword));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetServiceById(string id)
        {
            AdminServiceService adminServiceService = new AdminServiceService();
            return Success(adminServiceService.GetServiceById(id));
        }
        [HttpPost]
        public JsonResult InsertService(ServiceDental model)
        {
            try
            {
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        AdminServiceService adminServiceService = new AdminServiceService(connection);
                        ServiceDental service = new ServiceDental();
                        service.ServiceId = Guid.NewGuid().ToString();
                        service.Name = model.Name;
                        service.SearchName = model.SearchName;
                        service.ServiceCategoryId = model.ServiceCategoryId;
                        service.Description = model.Description;
                        service.Price = model.Price;
                        service.ExpectTime = model.ExpectTime;
                        service.Enable = true;
                        service.CreateTime = HelperProvider.GetSeconds();
                        if (!string.IsNullOrEmpty(model.Thumbnail))
                        {
                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            var path = System.Web.HttpContext.Current.Server.MapPath(Constant.SERVICE_THUMBNAIL_PATH + filename);
                            HelperProvider.Base64ToImage(model.Thumbnail, path);
                            service.Thumbnail = Constant.SERVICE_THUMBNAIL_URL + filename;
                        }
                        if (!adminServiceService.InsertService(service, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
                        transaction.Commit();
                        return Success();
                    }
                }
            }

            catch (Exception Ex)
            {
                return Error(Ex.Message);
            }
        }


        [HttpPost]
        public JsonResult UpdateService(ServiceDental model)
        {
            try
            {
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        AdminServiceService adminService = new AdminServiceService(connection);
                        ServiceDental service = adminService.GetServiceById(model.ServiceId, transaction);
                        if (service == null) return Error("Dịch vụ này không tồn tại.");
                        service.ServiceCategoryId = model.ServiceCategoryId;
                        service.Name = model.Name;
                        service.SearchName = model.SearchName;
                        service.Description = model.Description;
                        service.Price = model.Price;
                        service.ExpectTime = model.ExpectTime;
                        if (!string.IsNullOrEmpty(model.Thumbnail))
                        {
                            //xóa file cũ
                            if (!HelperProvider.DeleteFile(service.Thumbnail)) return Error(JsonResult.Message.ERROR_SYSTEM);
                            //tạo file mới
                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            var path = System.Web.HttpContext.Current.Server.MapPath(Constant.SERVICE_THUMBNAIL_PATH + filename);
                            HelperProvider.Base64ToImage(model.Thumbnail, path);
                            service.Thumbnail = Constant.SERVICE_THUMBNAIL_URL + filename;
                        }
                        if (!adminService.UpdateService(service, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
                        transaction.Commit();
                        return Success();
                    }
                }
            }

            catch (Exception Ex)
            {
                return Error(Ex.Message);
            }
        }
        [HttpGet]
        public JsonResult DeleteService(string serviceId)
        {
            try
            {
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        AdminServiceService adminService = new AdminServiceService(connection);
                        ServiceDental service = adminService.GetServiceById(serviceId, transaction);
                        if (service == null) return Error("Dịch vụ này không tồn tại.");
                        if (!string.IsNullOrEmpty(service.Thumbnail))
                        {
                            if (!HelperProvider.DeleteFile(service.Thumbnail)) return Error(JsonResult.Message.ERROR_SYSTEM);
                        }
                        if (!adminService.DeleteService(serviceId, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
                        transaction.Commit();
                        return Success();
                    }
                }
            }

            catch (Exception Ex)
            {
                return Error(Ex.Message);
            }
        }
    }
}
