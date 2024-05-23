using Dapper;
using DentalClinic.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DentalClinic.Services
{
    public class UserAdminService : BaseService
    {
        public UserAdminService() : base() { }
        public UserAdminService(IDbConnection db) : base(db) { }

        public UserAdmin GetUserAdminByToken(string token, IDbTransaction transaction = null)
        {
            string query = "select * from [user_admin] where Token = @token";
            return this._connection.Query<UserAdmin>(query, new { token }, transaction).FirstOrDefault();
        }
        public UserAdmin GetUserAdminByAccount(string account, IDbTransaction transaction = null)
        {
            string query = "select * from user_admin where Account=@account";
            return this._connection.Query<UserAdmin>(query, new { account = account }, transaction).FirstOrDefault();
        }
        public bool UpdateUserAdminToken(string userAdminId, string token, IDbTransaction transaction = null)
        {
            string query = "update user_admin set Token=@token where UserAdminId=@userAdminId";
            int status = this._connection.Execute(query, new { userAdminId, token }, transaction);
            return status > 0;
        }
        public bool InsertUserAdmin(UserAdmin model, IDbTransaction transaction = null)
        {
            string insert = "INSERT INTO [dbo].[user_admin] ([UserAdminId],[Account],[Name],[Password],[Token],[CreateTime],[Role]) VALUES (@UserAdminId,@Account,@Name,@Password,@Token,@CreateTime,@Role)";
            int status = this._connection.Execute(insert, model, transaction);
            return status > 0;
        }
        public UserAdmin GetUserAdminById(string adminId, IDbTransaction transaction = null)
        {
            string query = "select * from user_admin where UserAdminId=@adminId";
            return this._connection.Query<UserAdmin>(query, new { adminId = adminId }, transaction).FirstOrDefault();
        }
        public bool UpdateUserAdmin(UserAdmin model, IDbTransaction transaction = null)
        {
            string update = "UPDATE [dbo].[user_admin] SET [Account] = @Account,[Name] = @Name,[Password] = @Password,[Role]=@Role WHERE [UserAdminId] = @UserAdminId";
            int status = this._connection.Execute(update, model, transaction);
            return status > 0;
        }
        public bool DeleteUserAdmin(string adminId, IDbTransaction transaction = null)
        {
            string delete = "DELETE FROM [dbo].[user_admin] WHERE [UserAdminId] = @adminId";
            int status = this._connection.Execute(delete, new { adminId = adminId }, transaction);
            return status > 0;
        }
        public List<object> GetListUserAdmin(IDbTransaction transaction = null)
        {
            string query = "select UserAdminId, Account, Name, Role from user_admin";
            return this._connection.Query<object>(query, null, transaction).ToList();
        }
        public bool UpdateUserAdminPassword(string userAdminId, string password, IDbTransaction transaction = null)
        {
            string query = "update user_admin set Password=@password where UserAdminId=@userAdminId";
            int status = this._connection.Execute(query, new { userAdminId, password }, transaction);
            return status > 0;
        }
    }
}