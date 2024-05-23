using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DentalClinic.Areas.Admin.Controllers
{
    public class ManageDoctorController : Controller
    {
        // GET: Admin/ManageDoctor
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddDoctor()
        {
            return View();
        }
        public ActionResult UpdateDoctor(string id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}