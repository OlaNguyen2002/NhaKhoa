using DentalClinic.Models;
using DentalClinic.Providers;
using DentalClinic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace DentalClinic.Areas.Admin.ApiControllers
{
    public class AdminSlideController : ApiAdminBaseController
    {
        [HttpGet]
        public JsonResult GetListSlide(int? page, string keyword = "")
        {
            try
            {
                AdminSlideService adminSlideService = new AdminSlideService();
                if (!page.HasValue) page = 1;
                return Success(adminSlideService.GetListSlide(page.Value, keyword));
            }
            catch (Exception ex)
            {
                return Error();
            }
        }
        [HttpGet]
        public JsonResult GetSlideById(string id)
        {
            AdminSlideService adminSlideService = new AdminSlideService();
            return Success(adminSlideService.GetSlideById(id));
        }

        [HttpPost]
        public JsonResult InsertSlide(Slide model)
        {
            try
            {
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        AdminSlideService adminSlideService = new AdminSlideService(connection);
                        Slide slide = new Slide();
                        slide.SlideId = Guid.NewGuid().ToString();
                        slide.LinkWeb = model.LinkWeb;
                        if (model.Area == "banner-1") slide.Area = Slide.EnumArea.BANNER_AREA_1;
                        else if (model.Area == "home-slide") slide.Area = Slide.EnumArea.HOME_SLIDE;
                        else return Error();
                        if (!string.IsNullOrEmpty(model.Image))
                        {
                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            var path = System.Web.HttpContext.Current.Server.MapPath(Constant.SLIDE_IMAGE_PATH + filename);
                            HelperProvider.Base64ToImage(model.Image, path);
                            slide.Image = Constant.SLIDE_IMAGE_URL + filename;
                        }
                        if (!adminSlideService.InsertSlide(slide, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
                        transaction.Commit();
                        return Success();
                    }
                }
            }

            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult UpdateSlide(Slide model)
        {
            try
            {
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        AdminSlideService adminSlideService = new AdminSlideService(connection);
                        Slide slide = adminSlideService.GetSlideById(model.SlideId, transaction);
                        if (slide == null) return Error("Slide này không tồn tại.");
                        slide.LinkWeb = model.LinkWeb;
                        if (model.Area == "banner-1") slide.Area = Slide.EnumArea.BANNER_AREA_1;
                        else if (model.Area == "home-slide") slide.Area = Slide.EnumArea.HOME_SLIDE;
                        else return Error();
                        if (!string.IsNullOrEmpty(model.Image))
                        {
                            //xóa file cũ
                            if (!HelperProvider.DeleteFile(slide.Image)) return Error(JsonResult.Message.ERROR_SYSTEM);
                            //tạo file mới
                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            var path = System.Web.HttpContext.Current.Server.MapPath(Constant.SLIDE_IMAGE_PATH + filename);
                            HelperProvider.Base64ToImage(model.Image, path);
                            slide.Image = Constant.SLIDE_IMAGE_URL + filename;
                        }
                        if (!adminSlideService.UpdateSlide(slide, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
                        transaction.Commit();
                        return Success();
                    }
                }
            }

            catch (Exception Ex)
            {
                return Error();
            }
        }

        [HttpGet]
        public JsonResult DeleteSlide(string slideId)
        {
            try
            {
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        AdminSlideService adminSlideService = new AdminSlideService(connection);
                        Slide slide = adminSlideService.GetSlideById(slideId, transaction);
                        if (slide == null) return Error("Slide này không tồn tại.");
                        if (!string.IsNullOrEmpty(slide.Image))
                        {
                            if (!HelperProvider.DeleteFile(slide.Image)) return Error(JsonResult.Message.ERROR_SYSTEM);
                        }
                        if (!adminSlideService.DeleteSlide(slideId, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
                        transaction.Commit();
                        return Success();
                    }
                }
            }

            catch (Exception Ex)
            {
                return Error();
            }
        }
    }
}
