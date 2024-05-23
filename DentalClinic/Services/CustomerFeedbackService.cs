using Dapper;
using DentalClinic.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DentalClinic.Services
{
    public class CustomerFeedbackService : BaseService
    {
        public CustomerFeedbackService() : base() { }
        public CustomerFeedbackService(IDbConnection db) : base(db) { }

        public List<CustomerFeedback> GetListCustomerFeedback(IDbTransaction transaction = null)
        {
            string query = "Select * from customer_feedback";
            return this._connection.Query<CustomerFeedback>(query, transaction).ToList();
        }
        public CustomerFeedback GetFeedbackById(int CustomerFeedbackId, IDbTransaction transaction = null)
        {
            string query = "Select * FROM [dbo].[customer_feedback] WHERE CustomerFeedbackId=@CustomerFeedbackId";
            return this._connection.Query<CustomerFeedback>(query, new { CustomerFeedbackId }, transaction).FirstOrDefault();
        }
        public bool InsertFeedback(CustomerFeedback model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[customer_feedback]([CustomerName],[Avatar],[Contents]) VALUES(@CustomerName,@Avatar,@Contents)";
            int status = this._connection.Execute(query, model, transaction);
            return status > 0;
        }
        public bool DeleteFeedback(int CustomerFeedbackId, IDbTransaction transaction = null)
        {
            string query = "DELETE FROM [dbo].[customer_feedback] WHERE CustomerFeedbackId=@CustomerFeedbackId";
            int status = this._connection.Execute(query, new { CustomerFeedbackId }, transaction);
            return status > 0;
        }
        public bool UpdateFeedback(CustomerFeedback model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[customer_feedback] SET [CustomerName] = @CustomerName,[Avatar] = @Avatar,[Contents] = @Contents  WHERE CustomerFeedbackId=@CustomerFeedbackId";
            int status = this._connection.Execute(query, model, transaction);
            return status > 0;
        }
    }
}