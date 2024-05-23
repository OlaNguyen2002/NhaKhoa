using Dapper;
using DentalClinic.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DentalClinic.Services
{
    public class AdminServiceCategoryService
    {
        protected IDbConnection _connection;

        public AdminServiceCategoryService()
        {
            string constr = ConfigurationManager.ConnectionStrings["DESKTOP-PG31PRQ"].ToString();
            this._connection = new SqlConnection(constr);
        }

        public AdminServiceCategoryService(IDbConnection _connection)
        {
            if (_connection == null)
            {
                string constr = ConfigurationManager.ConnectionStrings["DESKTOP-PG31PRQ"].ToString();
                this._connection = new SqlConnection(constr);
            }
            else
                this._connection = _connection;
        }


        public List<ServiceCategory> GetListServiceCateGory()
        {
            string query = "select * from service_category";
            return this._connection.Query<ServiceCategory>(query).ToList();
        }

        public int InsertServiceCategory(ServiceCategory model, IDbTransaction transaction = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("ServiceCategoryId", model.ServiceCategoryId, DbType.String);
            parameters.Add("Name", model.Name, DbType.String);
            parameters.Add("Enable", model.Enable, DbType.Boolean);
            parameters.Add("ShowInHomePage", model.ShowInHomePage, DbType.Boolean);
            string query = "insert into service_category (ServiceCategoryId, Name, Enable, ShowInHomePage) values (@ServiceCategoryId, @Name, @Enable, @ShowInHomePage)";
            var numberOfEffectedRows = this._connection.Execute(query, parameters, transaction);
            return numberOfEffectedRows;
        }

        public ServiceCategory GetServiceCategoryById(string ServiceCategoryId, IDbTransaction transaction = null)
        {
            string query = "select * from service_category where ServiceCategoryId = @ServiceCategoryId";
            return this._connection.Query<ServiceCategory>(query, new { ServiceCategoryId }, transaction).FirstOrDefault();
        }

        public int UpdateServiceCategory(ServiceCategory model, IDbTransaction transaction = null)
        {
            string query = "update service_category set Name = @Name, Enable = @Enable, ShowInHomePage = @ShowInHomePage where ServiceCategoryId = @ServiceCategoryId";
            var numberOfEffectedRows = this._connection.Execute(query, model, transaction);
            return numberOfEffectedRows;
        }

        public ServiceCategory DeleteServiceCategory(string ServiceCategoryId, IDbTransaction transaction = null)
        {
            string query = "delete from service_category where ServiceCategoryId = @ServiceCategoryId";
            return this._connection.Query<ServiceCategory>(query, new { ServiceCategoryId }, transaction).FirstOrDefault();
        }
    }
}