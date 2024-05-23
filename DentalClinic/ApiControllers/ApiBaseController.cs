using DentalClinic.Filters;
using DentalClinic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace DentalClinic.ApiControllers
{

    public class ApiBaseController : ApiController
    {
        public JsonResult Success(object data = null, string message = null)
        {
            return new JsonResult { status = JsonResult.Status.SUCCESS, data = data, message = message };
        }

        public JsonResult Error(string message = JsonResult.Message.ERROR_SYSTEM)
        {
            return new JsonResult { status = JsonResult.Status.ERROR, message = message };
        }

        public JsonResult Unauthorized()
        {
            return new JsonResult { status = JsonResult.Status.UNAUTHORIZED, message = JsonResult.Message.TOKEN_EXPIRED };
        }

    }
}
