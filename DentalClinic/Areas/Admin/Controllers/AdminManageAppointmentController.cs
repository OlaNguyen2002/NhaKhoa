using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DentalClinic.Areas.Admin.Controllers
{
    public class AdminManageAppointmentController : Controller
    {
        // GET: Admin/AdminManageAppointment
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddAppointment()
        {
            return View();
        }
        public ActionResult ManageAppointment()
        {
            return View();
        }
        public ActionResult Detail(string userAppointmentId)
        {
            ViewBag.UserAppointmentId = userAppointmentId;
            return View();
        }
    }
}