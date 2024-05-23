using DentalClinic.Models;
using DentalClinic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DentalClinic.Controllers
{
    public class DoctorController : Controller
    {
        // GET: Doctor
        [Route("bac-si/danh-sach/{page?}")]
        public ActionResult Index(int page = 1)
        {
            DoctorService doctorService = new DoctorService();
            DoctorListViewModel lsDoctor = doctorService.GetListDoctorForListDoctor(page);
            ViewBag.ListDoctor = lsDoctor;
            ViewBag.Page = page;
            return View();
        }
        [Route("bac-si/bai-viet/{slug}/{id}")]
        public ActionResult Detail(string id)
        {

            DoctorService doctorService = new DoctorService();
            Doctor doctorUpdate = doctorService.GetDoctorDetail(id);
            if (doctorUpdate == null) throw new Exception();
            ViewBag.Doctor = doctorUpdate;

            // Lấy danh sách liên quan
            List<Doctor> lsDoctorUpdate = doctorService.GetListRelatedDoctor();
            ViewBag.ListRelatedDoctor = lsDoctorUpdate;
            return View();
        }
    }
}