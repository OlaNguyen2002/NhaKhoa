using DentalClinic.Models;
using DentalClinic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DentalClinic.Controllers
{
    public class NewsController : Controller
    {
        // GET: News
        [Route("tin-tuc/danh-sach/{page?}")]
        public ActionResult Index(int page = 1)
        {
            NewsService newsService = new NewsService();
            NewsListViewModel lsNews = newsService.GetListNewsForListNews(page);
            ViewBag.ListNews = lsNews;
            ViewBag.Page = page;
            return View();
        }
        [Route("tin-tuc/bai-viet/{slug}/{id}")]
        public ActionResult Detail(string id)
        {
            NewsService newsService = new NewsService();
            News newsUpdate = newsService.GetNewsDetail(id);
            if (newsUpdate == null) throw new Exception();
            ViewBag.News = newsUpdate;

            // Lấy danh sách liên quan
            List<News> lsNewsUpdate = newsService.GetListRelatedNews();
            ViewBag.ListRelatedNews = lsNewsUpdate;
            return View();
        }
    }
}