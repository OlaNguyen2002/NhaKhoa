using DentalClinic.Models;
using DentalClinic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DentalClinic.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            // Danh sách 10 tin tức mới nhất
            NewsService newsService = new NewsService();
            List<News> lsnewsUpdates = newsService.GetListNewsForHomePage();
            ViewBag.ListNews = lsnewsUpdates;

            // Danh sách 10 bac si noi bat nhat
            DoctorService doctorService = new DoctorService();
            List<Doctor> lsDoctorUpdates = doctorService.GetListDoctorForHomePage();
            ViewBag.ListDoctor = lsDoctorUpdates;

            // Danh sách 10 dich vu noi bat nhat
            ServiceService  serviceService = new ServiceService();
            List<ServiceDental> lsServiceUpdates = serviceService.GetListServiceForHomePage();
            ViewBag.ListService = lsServiceUpdates;

            // Danh sách phan hoi khach hang
            CustomerFeedbackService customerFeedbackService = new CustomerFeedbackService();
            List<CustomerFeedback> lsCustomerFeedbackUpdates = customerFeedbackService.GetListCustomerFeedback();
            ViewBag.ListCustomerFeedback = lsCustomerFeedbackUpdates;
            return View();
        }
        [Route("gioi-thieu")]
        public ActionResult About()
        {
            SystemAboutService systemAboutService = new SystemAboutService();
            SystemAbout systemAbout = systemAboutService.GetSystemAbout();
            ViewBag.SystemAbout = systemAbout;
            return View();
        }
        [Route("lien-he")]
        public ActionResult Contact()
        {
            return View();
        }
        [Route("dang-nhap")]
        public ActionResult LogIn()
        {
            return View();
        }
        [Route("dang-ky")]
        public ActionResult Register()
        {
            return View();
        }
        [Route("dat-lich-hen")]
        public ActionResult GetAppointment()
        {
            return View();
        }
        [Route("lich-hen-user")]
        public ActionResult UserAppointment()
        {
            return View();
        }
        [Route("chi-tiet-lich-hen/{id}")]
        public ActionResult AppointmentInfoDetail(string id)
        {
            ViewBag.Id = id;
            return View();
        }
        [Route("doi-mat-khau")]
        public ActionResult ChangePassword()
        {
            return View();
        }
        [Route("thong-tin-ca-nhan")]
        public ActionResult UserInformation()
        {
            return View();
        }
        

    }
}