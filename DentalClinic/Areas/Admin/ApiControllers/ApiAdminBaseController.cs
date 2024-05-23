using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DentalClinic.ApiControllers;
using DentalClinic.Filters;

namespace DentalClinic.Areas.Admin.ApiControllers
{
    [APIAdminToken]
    public class ApiAdminBaseController : ApiBaseController
    {
    }
}
