using DentalClinic.Models;
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
    public class AdminServiceCategoryController : ApiAdminBaseController
    {
        [HttpGet]
        //[ApiAdminTokenRequire]
        public JsonResult GetListServiceCateGory()
        {
            try
            {
                AdminServiceCategoryService adminServiceCategoryService = new AdminServiceCategoryService();
                //UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                //if (userAdmin == null) return Unauthorized();
                if (adminServiceCategoryService.GetListServiceCateGory() != null)
                {
                    return Success(adminServiceCategoryService.GetListServiceCateGory(), "Lấy dữ liệu thành công!");
                }
                else
                {
                    return Error();
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        //[ApiAdminTokenRequire]
        public JsonResult InsertServiceCategory(ServiceCategory model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        AdminServiceCategoryService adminServiceCategoryService = new AdminServiceCategoryService(connect);
                        //UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                        //if (userAdmin == null) return Unauthorized();

                        ServiceCategory serviceCategory = new ServiceCategory();
                        serviceCategory.ServiceCategoryId = Guid.NewGuid().ToString();
                        serviceCategory.Name = model.Name;
                        serviceCategory.Enable = true;
                        serviceCategory.ShowInHomePage = model.ShowInHomePage;

                        if (adminServiceCategoryService.InsertServiceCategory(serviceCategory, transaction) < 0) return Error();
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
        //[ApiAdminTokenRequire]
        public JsonResult GetServiceCategoryById(string ServiceCategoryId)
        {
            try
            {
                //UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                //if (userAdmin == null) return Unauthorized();

                AdminServiceCategoryService adminServiceCategoryService = new AdminServiceCategoryService();
                if (adminServiceCategoryService.GetServiceCategoryById(ServiceCategoryId) != null)
                {
                    return Success(adminServiceCategoryService.GetServiceCategoryById(ServiceCategoryId), "Lấy dữ liệu thành công!");
                }
                else
                {
                    return Error("Không tìm thấy service category");
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        //[ApiAdminTokenRequire]
        public JsonResult UpdateServiceCategory(ServiceCategory model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        AdminServiceCategoryService adminServiceCategoryService = new AdminServiceCategoryService(connect);
                        //UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                        //if (userAdmin == null) return Unauthorized();
                        //kiem tra nguoi dung co ton tai khong
                        if (adminServiceCategoryService.GetServiceCategoryById(model.ServiceCategoryId, transaction) == null)
                        {
                            return Error("Không tìm thấy service Category");
                        }
                        //nếu tồn tại thì tiến hành sửa
                        else
                        {
                            ServiceCategory serviceCategory = new ServiceCategory();
                            serviceCategory.ServiceCategoryId = model.ServiceCategoryId;
                            serviceCategory.Name = model.Name;
                            serviceCategory.ShowInHomePage = model.ShowInHomePage;
                            if (adminServiceCategoryService.UpdateServiceCategory(serviceCategory, transaction) < 0) return Error("Sửa thất bại!");
                            adminServiceCategoryService.UpdateServiceCategory(serviceCategory, transaction);
                            transaction.Commit();
                            return Success(null, "Sửa thành công!");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        //[ApiAdminTokenRequire]
        public JsonResult DeleteServiceCategory(string ServiceCategoryId)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        AdminServiceCategoryService adminServiceCategoryService = new AdminServiceCategoryService(connect);
                        //UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                        //if (userAdmin == null) return Unauthorized();

                        //kiem tra nguoi dung co ton tai khong
                        if (adminServiceCategoryService.GetServiceCategoryById(ServiceCategoryId, transaction) == null) return Error("Không tìm thấy Service Category");

                        //nếu tồn tại thì tiến hành xoá
                        adminServiceCategoryService.DeleteServiceCategory(ServiceCategoryId, transaction);
                        transaction.Commit();
                        return Success(null, "Xoá thành công!");
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
