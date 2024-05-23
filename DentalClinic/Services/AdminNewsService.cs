using Dapper;
using DentalClinic.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DentalClinic.Services.Admin
{
    public class AdminNewsService : BaseService
    {
        public AdminNewsService() : base() { }

        public AdminNewsService(IDbConnection db) : base(db) { }

        public NewsListViewModel GetListNews(int page, string keyword, IDbTransaction transaction = null)
        {
            NewsListViewModel newsListViewModel = new NewsListViewModel();
            newsListViewModel.ListNews = new List<News>();
            newsListViewModel.TotalPage = 0;

            string querySelect = "select * ";
            string queryCount = "select count(*) as Total ";
            string query = " from [news] p";

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(' ', '%') + "%";
                query += " where SearchTitle like @keyword";
            }

            int totalRow = _connection.Query<int>(queryCount + query, new { keyword = keyword }, transaction).FirstOrDefault();
            if (totalRow > 0)
            {
                newsListViewModel.TotalPage = (int)Math.Ceiling((decimal)totalRow / Constant.ADMIN_PAGE_SIZE);
            }

            int skip = (page - 1) * Constant.ADMIN_PAGE_SIZE;
            query += " order by NewsId desc offset " + skip + " rows fetch next " + Constant.ADMIN_PAGE_SIZE + " rows only";
            newsListViewModel.ListNews = this._connection.Query<News>(querySelect + query, new { keyword = keyword }, transaction).ToList();
            return newsListViewModel;
        }
        public News GetNewsById(int newsId, IDbTransaction transaction = null)
        {
            string query = "select * from news where NewsId=@newsId ";
            return _connection.Query<News>(query, new { newsId }, transaction).FirstOrDefault();
        }
        public bool InsertNews(News news, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[news] ([Title],[Slug],[SearchTitle],[Description],[Thumbnail],[NewsContent],[CreateTime],[Enable]) VALUES ( @Title, @Slug, @SearchTitle, @Description, @Thumbnail, @NewsContent, @CreateTime,@Enable)";
            return _connection.Execute(query, news, transaction) > 0;
        }
        public bool UpdateNews(News news, IDbTransaction transaction = null)
        {
            string query = "UPDATE [news] SET Title = @Title, Slug=@Slug, SearchTitle=@SearchTitle, Description=@Description, Thumbnail=@Thumbnail, NewsContent=@NewsContent, CreateTime=@CreateTime, Enable=@Enable WHERE NewsId = @NewsId";
            return _connection.Execute(query, news, transaction) > 0; ;
        }
        public bool DeleteNews(int newsId, IDbTransaction transaction = null)
        {
            string query = "DELETE FROM news where NewsId=@newsId";
            return _connection.Execute(query, new { newsId }, transaction) > 0;
        }
        public List<News> GetListTopNews(IDbTransaction transaction = null)
        {
            string query = "select TOP(4) NewsId, Title, Thumbnail,Description,CreateTime from News Order by CreateTime desc";
            return this._connection.Query<News>(query, new { enable = true }, transaction).ToList();
        }
    }
}