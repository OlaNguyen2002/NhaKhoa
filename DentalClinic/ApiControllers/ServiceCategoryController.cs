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
    public class ServiceCategoryController : ApiBaseController
    {

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetListServiceCategory()
        {
            try
            {
                ServiceCategoryService serviceCategoryService = new ServiceCategoryService();
                if (serviceCategoryService.GetListServiceCategoryUpdate() != null)
                {
                    return Success(serviceCategoryService.GetListServiceCategoryUpdate(), "Lấy dữ liệu thành công!");
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
