using Dapper;
using DentalClinic.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DentalClinic.Services
{
    public class ServiceService : BaseService
    {
        public ServiceService() : base() { }
        public ServiceService(IDbConnection db) : base(db) { }

        public List<ServiceDental> GetListService()
        {
            string query = "select * from [service_dental]";
            return this._connection.Query<ServiceDental>(query).ToList();
        }
        public List<ServiceDental> GetListServiceForHomePage(IDbTransaction transaction = null)
        {
            string query = "select TOP(6) * from [service_dental]";
            return this._connection.Query<ServiceDental>(query, transaction).ToList();
        }
        public ServiceDental GetServiceById(string ServiceId, IDbTransaction transaction = null)
        {
            string query = "select * from [service_dental] where ServiceId = @ServiceId";
            return this._connection.Query<ServiceDental>(query, new { ServiceId }, transaction).FirstOrDefault();
        }
        public List<ServiceDental> GetServiceByUserAppointmentId(string UserAppointmentId, IDbTransaction transaction = null)
        {
            string query = "select s.* from [user_appointment_service] uas right join [service_dental] s on uas.ServiceId = s.ServiceId where uas.UserAppointmentId = @UserAppointmentId";
            return this._connection.Query<ServiceDental>(query, new { UserAppointmentId }, transaction).ToList();
        }
        public ServiceListViewModel GetListService(ServiceSearchModel serviceSearchModel, IDbTransaction transaction = null)
        {
            string querySelect = "select * ";
            string queryCount = "select count(*) as Total ";
            string query = " from service_dental where Enable=@Enable";
            if (!string.IsNullOrEmpty(serviceSearchModel.KeyWord))
            {
                serviceSearchModel.KeyWord = "%" + serviceSearchModel.KeyWord.Replace(" ", "%") + "%";
                query += " and SearchName like @KeyWord";
            }
            if (serviceSearchModel.ServiceCategoryId != null)
            {
                query += " and ServiceCategoryId=@ServiceCategoryId";
            }

            ServiceListViewModel serviceListViewModel = new ServiceListViewModel();
            serviceListViewModel.ListService = new List<ServiceDental>();
            serviceListViewModel.TotalPage = 0;

            int totalRows = this._connection.Query<int>(queryCount + query, serviceSearchModel, transaction).FirstOrDefault();
            if (totalRows > 0)
            {
                serviceListViewModel.TotalPage = (int)Math.Ceiling((decimal)totalRows / Constant.PAGE_SIZE);
            }
            if (serviceSearchModel.CurrentPage == 0)
            {
                serviceSearchModel.CurrentPage = 1;
            }
            int skip = (serviceSearchModel.CurrentPage - 1) * Constant.PAGE_SIZE;
            if (serviceSearchModel.OrderBy == "TIME_ASC")
            {
                query += " order by CreateTime";
            }
            if (serviceSearchModel.OrderBy == null || serviceSearchModel.OrderBy == "" || serviceSearchModel.OrderBy != "TIME_ASC")
            {
                query += " order by CreateTime desc";
            }
            query += " offset " + skip + " rows fetch next " + Constant.PAGE_SIZE + " rows only";
            serviceListViewModel.ListService = this._connection.Query<ServiceDental>(querySelect + query, serviceSearchModel, transaction).ToList();
            return serviceListViewModel;
        }
        public List<ServiceDental> GetListServiceByServiceCategoryId(string serviceCategoryId, IDbTransaction transaction = null)
        {
            string query = "select * from service_dental where ServiceCategoryId=@serviceCategoryId and Enable=@enable";
            return this._connection.Query<ServiceDental>(query, new { serviceCategoryId, enable = "True" }, transaction).ToList();
        }
    }
}