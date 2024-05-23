using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DentalClinic.Controllers
{
    public class NotificationController : Controller
    {
        // GET: Notification
        [Route("thong-bao")]
        public ActionResult Index()
        {
            return View();
        }
    }
}