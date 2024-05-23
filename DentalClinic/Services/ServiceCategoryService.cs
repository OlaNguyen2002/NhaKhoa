using Dapper;
using DentalClinic.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DentalClinic.Services
{
    public class ServiceCategoryService : BaseService
    {
        public ServiceCategoryService() : base() { }
        public ServiceCategoryService(IDbConnection db) : base(db) { }
        public ServiceCategoryUpdate GetServiceCategoryUpdateById(string id, IDbTransaction transaction = null)
        {
            string query = "select * from [dbo].[service_category] where ServiceCategoryId=@id and Enable=1";
            return this._connection.Query<ServiceCategoryUpdate>(query, new { id }, transaction).FirstOrDefault();
        }
        public ServiceCategory GetServiceCategoryById(string id, IDbTransaction transaction = null)
        {
            string query = "select * from [dbo].[service_category] where ServiceCategoryId=@id";
            return this._connection.Query<ServiceCategory>(query, new { id }, transaction).FirstOrDefault();
        }
        public List<ServiceCategoryUpdate> GetListServiceCategoryUpdate(IDbTransaction transaction = null)
        {
            string query = "select * from [dbo].[service_category] where Enable=1 and ShowInHomePage=1";
            return this._connection.Query<ServiceCategoryUpdate>(query, null, transaction).ToList();
        }
        public List<ServiceCategory> GetListServiceCategoryHomePage(IDbTransaction transaction = null)
        {
            string query = "SELECT* FROM [dbo].[service_category] where Enable=@enable";

            return this._connection.Query<ServiceCategory>(query, new { enable = true }, transaction).ToList();
        }
    }
}