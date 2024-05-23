using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DentalClinic.Areas.Admin.Controllers
{
    public class CustomerFeedbackController : Controller
    {
        // GET: Admin/CustomerFeedback
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddFeedback()
        {
            return View();
        }
        public ActionResult UpdateFeedback(int id)
        {
            ViewBag.id = id;
            return View();
        }
    }
}