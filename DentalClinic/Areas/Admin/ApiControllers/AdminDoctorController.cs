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
    public class AdminDoctorController : ApiAdminBaseController
    {
        [HttpGet]
        public JsonResult GetListDoctor(int? page, string keyword = "")
        {
            try
            {
                AdminDoctorService adminDoctorService = new AdminDoctorService();
                if (!page.HasValue) page = 1;
                return Success(adminDoctorService.GetListDoctor(page.Value, keyword));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetDoctorById(string id)
        {
            AdminDoctorService adminDoctorService = new AdminDoctorService();
            return Success(adminDoctorService.GetDoctorById(id));
        }
        [HttpPost]
        public JsonResult InsertDoctor(Doctor model)
        {
            try
            {
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        AdminDoctorService adminDoctorService = new AdminDoctorService(connection);
                        Doctor doctor = new Doctor();
                        doctor.DoctorId = Guid.NewGuid().ToString();
                        doctor.Name = model.Name;
                        doctor.SearchName = model.SearchName;
                        doctor.Description = model.Description.Trim();
                        doctor.Qualification = model.Qualification;
                        doctor.CreateTime = HelperProvider.GetSeconds();
                        doctor.Enable = true;
                        if (!string.IsNullOrEmpty(model.Avatar))
                        {
                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            var path = System.Web.HttpContext.Current.Server.MapPath(Constant.DOCTOR_THUMBNAIL_PATH + filename);
                            HelperProvider.Base64ToImage(model.Avatar, path);
                            doctor.Avatar = Constant.DOCTOR_THUMBNAIL_URL+ filename;
                        }
                        if (!adminDoctorService.InsertDoctor(doctor, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
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
        public JsonResult UpdateDoctor(Doctor model)
        {
            try
            {
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        AdminDoctorService adminDoctorService = new AdminDoctorService(connection);
                        Doctor doctor = adminDoctorService.GetDoctorById(model.DoctorId, transaction);
                        if (doctor == null) return Error("Bác sĩ này không tồn tại.");
                        doctor.Name = model.Name;
                        doctor.SearchName = model.SearchName;
                        doctor.Description = model.Description;
                        doctor.Qualification = model.Qualification;
                        if (!string.IsNullOrEmpty(model.Avatar))
                        {
                            //xóa file cũ
                            if (!HelperProvider.DeleteFile(doctor.Avatar)) return Error(JsonResult.Message.ERROR_SYSTEM);
                            //tạo file mới
                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            var path = System.Web.HttpContext.Current.Server.MapPath(Constant.DOCTOR_THUMBNAIL_PATH+ filename);
                            HelperProvider.Base64ToImage(model.Avatar, path);
                            doctor.Avatar = Constant.DOCTOR_THUMBNAIL_URL + filename;
                        }
                        if (!adminDoctorService.UpdateDoctor(doctor, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
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
        public JsonResult DeleteDoctor(string doctorId)
        {
            try
            {
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        AdminDoctorService adminDoctorService = new AdminDoctorService(connection);
                        Doctor doctor = adminDoctorService.GetDoctorById(doctorId, transaction);
                        if (doctor == null) return Error("Bác sĩ này không tồn tại.");
                        if (!string.IsNullOrEmpty(doctor.Avatar))
                        {
                            if (!HelperProvider.DeleteFile(doctor.Avatar)) return Error(JsonResult.Message.ERROR_SYSTEM);
                        }
                        if (!adminDoctorService.DeleteDoctor(doctorId, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
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
