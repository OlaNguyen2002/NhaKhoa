using Dapper;
using DentalClinic.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DentalClinic.Services
{
    public class AdminServiceService : BaseService
    {
        public AdminServiceService() : base() { }

        public AdminServiceService(IDbConnection db) : base(db) { }
        public ServiceListViewModel GetListService(int page, string keyword, IDbTransaction transaction = null)
        {
            ServiceListViewModel serviceListViewModel = new ServiceListViewModel();
            serviceListViewModel.ListService = new List<ServiceDental>();
            serviceListViewModel.TotalPage = 0;

            string querySelect = "select * ";
            string queryCount = "select count(*) as Total ";
            string query = " from [service_dental] ";

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(' ', '%') + "%";
                query += " where SearchName like @keyword";
            }

            int totalRow = _connection.Query<int>(queryCount + query, new { keyword = keyword }, transaction).FirstOrDefault();
            if (totalRow > 0)
            {
                serviceListViewModel.TotalPage = (int)Math.Ceiling((decimal)totalRow / Constant.ADMIN_PAGE_SIZE);
            }

            int skip = (page - 1) * Constant.ADMIN_PAGE_SIZE;
            query += " order by CreateTime desc offset " + skip + " rows fetch next " + Constant.ADMIN_PAGE_SIZE + " rows only";
            serviceListViewModel.ListService = this._connection.Query<ServiceDental>(querySelect + query, new { keyword = keyword }, transaction).ToList();
            return serviceListViewModel;
        }
        public ServiceDental GetServiceById(string serviceId, IDbTransaction transaction = null)
        {
            string query = "select * from service_dental where ServiceId=@serviceId ";
            return _connection.Query<ServiceDental>(query, new { serviceId }, transaction).FirstOrDefault();
        }
        public bool InsertService(ServiceDental service, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[service_dental] ([ServiceId],[Name],[SearchName],[Thumbnail],[ExpectTime],[Description],[ServiceCategoryId],[Price],[CreateTime], [Enable]) VALUES ( @ServiceId, @Name, @SearchName, @Thumbnail, @ExpectTime, @Description, @ServiceCategoryId, @Price, @CreateTime, @Enable)";
            return _connection.Execute(query, service, transaction) > 0;
        }
        public bool UpdateService(ServiceDental service, IDbTransaction transaction = null)
        {
            string query = "UPDATE [service_dental] SET [Name]=@Name, [SearchName]=@SearchName, [Thumbnail]=@Thumbnail, [ExpectTime]=@ExpectTime, [Description]=@Description,[ServiceCategoryId]=@ServiceCategoryId,[Price]=@Price,[CreateTime]=@CreateTime, [Enable]=@Enable  WHERE ServiceId=@ServiceId";
            return _connection.Execute(query, service, transaction) > 0; ;
        }
        public bool DeleteService(string serviceId, IDbTransaction transaction = null)
        {
            string query = "DELETE FROM [service_dental] where ServiceId=@serviceId";
            return _connection.Execute(query, new { serviceId }, transaction) > 0;
        }
        public List<ServiceDental> GetListTopService(IDbTransaction transaction = null)
        {
            string query = "select TOP(4) ServiceId, Name, Thumbnail,Description, Price from service Order by CreateTime desc";
            return this._connection.Query<ServiceDental>(query, new { enable = true }, transaction).ToList();
        }
    }
}