using Dapper;
using DentalClinic.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DentalClinic.Services
{
    public class NewsService : BaseService
    {
        public NewsService() : base() { }
        public NewsService(IDbConnection db) : base(db) { }
        public List<News> GetListNewsForHomePage(IDbTransaction transaction = null)
        {
            string query = "select TOP(10) * from news Where Enable = 1 order by CreateTime DESC";
            return this._connection.Query<News>(query, new { enable = true }, transaction).ToList();
        }
        public NewsListViewModel GetListNewsForListNews(int page, IDbTransaction transaction = null)
        {
            string queryselect = "select * ";
            string querycount = "select count(*) as Total ";
            string query = " from news where Enable=@enable";

            NewsListViewModel newsListModel = new NewsListViewModel();
            newsListModel.ListNews = new List<News>();
            newsListModel.TotalPage = 0;

            int totalRows = this._connection.Query<int>(querycount + query, new { enable = true }, transaction).FirstOrDefault();
            if (totalRows > 0)
            {
                newsListModel.TotalPage = (int)Math.Ceiling((decimal)totalRows / Constant.PAGE_NEWS_SIZE);
            }
            if (page == 0)
            {
                page = 1;
            }
            int skip = (page - 1) * Constant.PAGE_NEWS_SIZE;
            query += " order by CreateTime desc";
            query += " offset " + skip + " rows fetch next " + Constant.PAGE_NEWS_SIZE + " rows only";
            newsListModel.ListNews = this._connection.Query<News>(queryselect + query, new { enable = true }, transaction).ToList();
            return newsListModel;
        }

        public News GetNewsDetail(string newsId, IDbTransaction transaction = null)
        {
            string query = "select * from [news] where Enable=@enable and NewsId=@newsId";
            return this._connection.Query<News>(query, new { enable = true, newsId }, transaction).FirstOrDefault();
        }

        public List<News> GetListRelatedNews(IDbTransaction transaction = null)
        {
            string query = "select TOP(6) * from news where Enable=@enable Order by CreateTime desc";
            return this._connection.Query<News>(query, new { enable = true }, transaction).ToList();
        }
    }
}