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
    public class ManageServicesController : ApiBaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetListService()
        {
            try
            {
                ServiceService serviceService = new ServiceService();
                if (serviceService.GetListService() != null)
                {
                    return Success(serviceService.GetListService(), "Lấy dữ liệu thành công!");
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
