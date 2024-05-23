using Dapper;
using DentalClinic.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DentalClinic.Services
{
    public class AdminSlideService : BaseService
    {
        public AdminSlideService() : base() { }

        public AdminSlideService(IDbConnection db) : base(db) { }


        public SlideListViewModel GetListSlide(int page, string keyword, IDbTransaction transaction = null)
        {
            SlideListViewModel slideListViewModel = new SlideListViewModel();
            slideListViewModel.ListSlide = new List<Slide>();
            slideListViewModel.TotalPage = 0;

            string querySelect = "select * ";
            string queryCount = "select count(*) as Total ";
            string query = " from [slide] p";

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(' ', '%') + "%";
                query += " where LinkWeb like @keyword";
            }

            int totalRow = _connection.Query<int>(queryCount + query, new { keyword = keyword }, transaction).FirstOrDefault();
            if (totalRow > 0)
            {
                slideListViewModel.TotalPage = (int)Math.Ceiling((decimal)totalRow / Constant.ADMIN_PAGE_SIZE);
            }

            int skip = (page - 1) * Constant.ADMIN_PAGE_SIZE;
            query += " order by SlideId desc offset " + skip + " rows fetch next " + Constant.ADMIN_PAGE_SIZE + " rows only";
            slideListViewModel.ListSlide = this._connection.Query<Slide>(querySelect + query, new { keyword = keyword }, transaction).ToList();
            return slideListViewModel;
        }
        public Slide GetSlideById(string slideId, IDbTransaction transaction = null)
        {
            string query = "select * from slide where SlideId=@slideId ";
            return _connection.Query<Slide>(query, new { slideId }, transaction).FirstOrDefault();
        }
        public bool InsertSlide(Slide slide, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[slide] ([SlideId],[Image],[LinkWeb],[Area]) VALUES (@SlideId, @Image, @LinkWeb, @Area)";
            return _connection.Execute(query, slide, transaction) > 0;
        }
        public bool UpdateSlide(Slide slide, IDbTransaction transaction = null)
        {
            string query = "UPDATE [slide] SET Image = @Image, LinkWeb=@LinkWeb, Area=@Area WHERE SlideId = @SlideId";
            return _connection.Execute(query, slide, transaction) > 0;
        }
        public bool DeleteSlide(string slideId, IDbTransaction transaction = null)
        {
            string query = "DELETE FROM slide where SlideId=@slideId";
            return _connection.Execute(query, new { slideId }, transaction) > 0;
        }
    }
}