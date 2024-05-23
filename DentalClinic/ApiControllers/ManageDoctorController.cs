using DentalClinic.Areas.Admin.ApiControllers;
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
    public class ManageDoctorController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetListDoctor()
        {
            try
            {
                DoctorService doctorService = new DoctorService();
                if (doctorService.GetListDoctor() != null)
                {
                    return Success(doctorService.GetListDoctor(), "Lấy dữ liệu thành công!");
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
    }
}
