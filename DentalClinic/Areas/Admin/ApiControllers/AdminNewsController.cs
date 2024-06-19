using DentalClinic.Models;
using DentalClinic.Providers;
using DentalClinic.Services;
using DentalClinic.Services.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace DentalClinic.Areas.Admin.ApiControllers
{
    public class AdminNewsController : ApiAdminBaseController
    {
        [HttpGet]
        public JsonResult GetListNews(int? page, string keyword = "")
        {
            try
            {
                AdminNewsService adminNewsService = new AdminNewsService();
                if (!page.HasValue) page = 1;
                return Success(adminNewsService.GetListNews(page.Value, keyword));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetNewsById(int id)
        {
            AdminNewsService adminNewsService = new AdminNewsService();
            return Success(adminNewsService.GetNewsById(id));
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult InsertNews(News model)
        {
            try
            {
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        AdminNewsService adminNewsService = new AdminNewsService(connection);
                        News news = new News();
                        news.Title = model.Title.Trim();
                        news.Description = model.Description;
                        news.NewsContent = model.NewsContent;
                        news.SearchTitle = model.SearchTitle.Trim();
                        if (!string.IsNullOrEmpty(model.Thumbnail))
                        {
                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            var path = System.Web.HttpContext.Current.Server.MapPath(Constant.NEWS_THUMBNAIL_PATH + filename);
                            HelperProvider.Base64ToImage(model.Thumbnail, path);
                            news.Thumbnail = Constant.NEWS_THUMBNAIL_URL + filename;
                        }
                        news.CreateTime = HelperProvider.GetSeconds();
                        news.Enable = true;
                        if (!adminNewsService.InsertNews(news, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
                        transaction.Commit();
                        return Success();
                    }
                }
            }

            catch (Exception Ex)
            {
                return Error(Ex.Message);
            }
        }

        [HttpPost]
        public JsonResult UpdateNews(News model)
        {
            try
            {
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        AdminNewsService adminNewsService = new AdminNewsService(connection);
                        News news = adminNewsService.GetNewsById(model.NewsId, transaction);
                        if (news == null) return Error("Tin tức này không tồn tại.");
                        news.Title = model.Title;
                        news.Description = model.Description;
                        news.NewsContent = model.NewsContent;
                        if (!string.IsNullOrEmpty(model.Thumbnail))
                        {
                            //xóa file cũ
                            if (!HelperProvider.DeleteFile(news.Thumbnail)) return Error(JsonResult.Message.ERROR_SYSTEM);
                            //tạo file mới
                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            var path = System.Web.HttpContext.Current.Server.MapPath(Constant.NEWS_THUMBNAIL_PATH + filename);
                            HelperProvider.Base64ToImage(model.Thumbnail, path);
                            news.Thumbnail = Constant.NEWS_THUMBNAIL_URL + filename;
                        }
                        if (!adminNewsService.UpdateNews(news, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
                        transaction.Commit();
                        return Success();
                    }
                }
            }

            catch (Exception Ex)
            {
                return Error(Ex.Message);
            }
        }

        [HttpGet]
        public JsonResult DeleteNews(int newsId)
        {
            try
            {
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        AdminNewsService adminNewsService = new AdminNewsService(connection);
                        News news = adminNewsService.GetNewsById(newsId, transaction);
                        if (news == null) return Error("Tin tức này không tồn tại.");
                        if (!string.IsNullOrEmpty(news.Thumbnail))
                        {
                            if (!HelperProvider.DeleteFile(news.Thumbnail)) return Error(JsonResult.Message.ERROR_SYSTEM);
                        }
                        if (!adminNewsService.DeleteNews(newsId, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
                        transaction.Commit();
                        return Success();
                    }
                }
            }

            catch (Exception Ex)
            {
                return Error(Ex.Message);
            }
        }
    }
}
