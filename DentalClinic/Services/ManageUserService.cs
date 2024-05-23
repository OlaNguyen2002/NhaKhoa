using Dapper;
using DentalClinic.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DentalClinic.Services
{
    public class ManageUserService : BaseService
    {
        public ManageUserService() : base() { }
        public ManageUserService(IDbConnection db) : base(db) { }

        public UserListViewModel GetListUser(int page, string keyword, bool? enable = null, IDbTransaction transaction = null)
        {
            UserListViewModel userListViewModel = new UserListViewModel();
            userListViewModel.ListUser = new List<User>();
            userListViewModel.TotalPage = 0;

            string querySelect = "select * ";
            string queryCount = "select count(*) as Total ";
            string query = " from [user] where 1=1";

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(' ', '%') + "%";
                query += " and SearchName like @keyword";
            }
            if(enable != null)
            {
                query += " and Enable=@enable";
            }
            int totalRow = _connection.Query<int>(queryCount + query, new { keyword = keyword, enable }, transaction).FirstOrDefault();
            if (totalRow > 0)
            {
                userListViewModel.TotalPage = (int)Math.Ceiling((decimal)totalRow / Constant.ADMIN_PAGE_SIZE);
            }

            int skip = (page - 1) * Constant.ADMIN_PAGE_SIZE;
            query += " order by CreateTime desc offset " + skip + " rows fetch next " + Constant.ADMIN_PAGE_SIZE + " rows only";
            userListViewModel.ListUser = this._connection.Query<User>(querySelect + query, new { keyword = keyword, enable }, transaction).ToList();
            return userListViewModel;
        }

        public object GetUserById(string userId, IDbTransaction transaction = null)
        {
            string query = "select * from [user] where UserId = @userId";
            return this._connection.Query<object>(query, new { userId }, transaction).FirstOrDefault();
        }

        //
        //

        public bool CheckAccountExist(string account, string userId, IDbTransaction transaction = null)
        {
            string query = "select count(*) from [user] where Account = @account and Account <> '' and UserId <> @userId";
            int count = this._connection.Query<int>(query, new { account = account, userId = userId }, transaction).FirstOrDefault();
            return count > 0;
        }
        public bool CheckAccountEmail(string email, string userId, IDbTransaction transaction = null)
        {
            string query = "select count(*) from [user] where Email = @email and Email <> '' and UserId <> @userId";
            int count = this._connection.Query<int>(query, new { email = email, userId = userId }, transaction).FirstOrDefault();
            return count > 0;
        }
        public bool CheckPhoneExist(string phone, string userId, IDbTransaction transaction = null)
        {
            string query = "select count(*) from [user] where Phone = @phone and Phone <> '' and UserId <> @userId";
            int count = this._connection.Query<int>(query, new { phone = phone, userId = userId }, transaction).FirstOrDefault();
            return count > 0;
        }
        public bool UpdateUser(User model, IDbTransaction transaction = null)
        {
            string update = "UPDATE [dbo].[user] SET [Name] = @Name, [SearchName] = @SearchName, [Avatar] = @Avatar,[Account] = @Account,[Email] = @Email,[Phone] = @Phone,[Password] = @Password," +
                            " [Address] = @Address,[Gender] = @Gender WHERE [UserId] = @UserId";
            int status = this._connection.Execute(update, model, transaction);
            return status > 0;
        }
        public bool ChangeUserEnable(User model, IDbTransaction transaction = null)
        {
            string update = "UPDATE [dbo].[user] SET [Enable] = @Enable WHERE [UserId] = @UserId";
            int status = this._connection.Execute(update, model, transaction);
            return status > 0;
        }

        
    }
}