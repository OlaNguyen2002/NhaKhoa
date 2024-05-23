using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using DentalClinic.Models;
using DentalClinic.Providers;

namespace DentalClinic.Filters
{
    public class APIAdminTokenAttribute : ActionFilterAttribute
    {
        private HttpResponseMessage UnauthorizedResponse()
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            httpResponseMessage.StatusCode = HttpStatusCode.OK;
            httpResponseMessage.Content = new StringContent(new JsonResult() { message = JsonResult.Message.TOKEN_EXPIRED, status = JsonResult.Status.UNAUTHORIZED }.ToString(), System.Text.Encoding.UTF8, "application/json");
            return httpResponseMessage;
            
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Count > 0) { base.OnActionExecuting(actionContext); return; }
                if (actionContext.Request.Headers.Authorization == null) throw new Exception("");

                UserAdmin userAdmin = UserProvider.GetUserAdminFromRequestHeader(actionContext.Request);
                if (userAdmin == null) throw new Exception();

            }
            catch (Exception ex)
            {
                actionContext.Response = UnauthorizedResponse();
            }

            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}