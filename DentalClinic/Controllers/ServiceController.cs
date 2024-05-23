using DentalClinic.Models;
using DentalClinic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DentalClinic.Controllers
{
    public class ServiceController : Controller
    {
        [Route("dich-vu/danh-sach/{page?}")]
        // GET: Service
        public ActionResult Index(ServiceSearchModel searchModel)
        {
            ServiceCategoryService serviceCategoryService = new ServiceCategoryService();
            ServiceSearchModel serviceSearchModel = new ServiceSearchModel();
            serviceSearchModel.Enable = true;
            serviceSearchModel.CurrentPage = searchModel.CurrentPage;
            serviceSearchModel.ServiceCategoryId = searchModel.ServiceCategoryId;
            serviceSearchModel.KeyWord = searchModel.KeyWord;
            if (searchModel.ServiceCategoryId != null)
            {
                ServiceCategoryUpdate serviceCategoryUpdate = serviceCategoryService.GetServiceCategoryUpdateById(searchModel.ServiceCategoryId);
                ViewBag.Category = serviceCategoryUpdate;
            }
            ServiceService serviceService = new ServiceService();
            ServiceListViewModel serviceListViewModel = serviceService.GetListService(serviceSearchModel);
            ViewBag.ListService = serviceListViewModel;

            // danh sách tab danh mục dich vu ở trang chủ
            List<ServiceCategoryUpdate> lsServiceCategoryUpdate = serviceCategoryService.GetListServiceCategoryUpdate();
            ViewBag.ListServiceCategoryUpdate = lsServiceCategoryUpdate;

            return View();
        }

        [Route("dich-vu/chi-tiet/{slug}/{id}")]
        public ActionResult Detail(string id)
        {
            // chi tiết dich vu
            ServiceService serviceService = new ServiceService();
            ServiceDental serviceDental = serviceService.GetServiceById(id);
            if (serviceDental == null) throw new Exception();
            ViewBag.ServiceDental = serviceDental;

            // danh sách dich vu
            List<ServiceDental> lsServiceDental = serviceService.GetListServiceByServiceCategoryId(serviceDental.ServiceCategoryId);
            ViewBag.ListServiceDental = lsServiceDental;

            // lấy ra danh mục
            ServiceCategoryService serviceCategoryService = new ServiceCategoryService();
            ServiceCategory serviceCategory = serviceCategoryService.GetServiceCategoryById(serviceDental.ServiceCategoryId);
            ViewBag.ServiceCategory = serviceCategory;
            return View();
        }
    }
}