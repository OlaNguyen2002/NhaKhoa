using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using DentalClinic.Models;
using DentalClinic.Services;

namespace DentalClinic.Filters
{
    public class PermissionAttribute: ActionFilterAttribute
    {
        public string Permissions { get; set; }
        public PermissionAttribute(string permission)
        {
            Permissions = permission;
        }
        private HttpResponseMessage UnauthorizedResponse()
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            httpResponseMessage.StatusCode = HttpStatusCode.OK;
            httpResponseMessage.Content = new StringContent(new JsonResult() { message = JsonResult.Message.NO_PERMISSION, status = JsonResult.Status.UNAUTHENTICATED }.ToString(), System.Text.Encoding.UTF8, "application/json");
            return httpResponseMessage;
        }
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                if (actionContext.Request.Headers.Authorization == null) throw new Exception(); 
                string token = actionContext.Request.Headers.Authorization.ToString();
                UserAdminService userAdminService = new UserAdminService();
                UserAdmin userAdmin = userAdminService.GetUserAdminByToken(token);
                if (userAdmin == null) throw new Exception();
                if (userAdmin.Role != Permissions) throw new Exception(); 
            }
            catch (Exception ex)
            {
                actionContext.Response = UnauthorizedResponse();
            }
            base.OnActionExecuting(actionContext);
        }
    }
}