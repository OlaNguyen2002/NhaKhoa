using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DentalClinic.Areas.Admin.Controllers
{
    public class ManageNewsController : Controller
    {
        // GET: Admin/ManageNews
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddNews()
        {
            return View();
        }
        public ActionResult UpdateNews(int id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}