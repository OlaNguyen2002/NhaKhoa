using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using DentalClinic.Models;

namespace DentalClinic.Services
{
    public class NotificationService : BaseService
    {
        public NotificationService() : base() { }
        public NotificationService(IDbConnection db) : base(db) { }
        public List<Notification> GetListNotification(string userId, int page)
        {
            string query = "select * from notification where 1 = 1 and UserId=@userId order by NotificationId desc";
            query += " OFFSET(" + (page - 1) * Constant.USER_PAGE_SIZE + ") ROWS FETCH NEXT(" + Constant.USER_PAGE_SIZE + ") ROWS ONLY ";
            List<Notification> ls = this._connection.Query<Notification>(query, new { userId = userId }).ToList();
            return ls;
        }

        public List<Notification> GetListNewNotification(string userId)
        {
            string query = "select top 10 * from notification where 1 = 1 and UserId=@userId order by CreateTime desc";
            List<Notification> ls = this._connection.Query<Notification>(query, new { userId = userId }).ToList();
            return ls;
        }

        public bool InsertNotification(Notification notification, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[notification] ([UserId], [Title], [Message], [IsRead], [CreateTime]) " +
                                "VALUES (@UserId, @Title, @Message, @IsRead, @CreateTime)";
            int status = this._connection.Execute(query, notification, transaction);
            return status > 0;
        }

        public bool UpdateNotificationRead(Notification model, IDbTransaction transaction)
        {
            string query = "UPDATE [dbo].[notification] SET IsRead = @IsRead WHERE NotificationId = @NotificationId";
            int status = this._connection.Execute(query, model, transaction);
            return status > 0;
        }

        public Notification GetNotificationById(long id, IDbTransaction transaction = null)
        {
            string query = "select * from notification where NotificationId = @id";
            return this._connection.Query<Notification>(query, new { id }, transaction).FirstOrDefault();
        }

        public int GetNumberNotificationForHomePage(string userId, IDbTransaction transaction = null)
        {
            string query = "select count(*) from notification where UserId=@userId and IsRead = @isRead";
            int count = this._connection.Query<int>(query, new { userId, isRead = false }, transaction).FirstOrDefault();
            return count;
        }
    }
}