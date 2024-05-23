using Dapper;
using DentalClinic.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DentalClinic.Services
{
    public class UserService : BaseService
    {
        public UserService() : base() { }
        public UserService(IDbConnection db) : base(db) { }
        public List<User> GetListUser(IDbTransaction transaction = null)
        {
            string query = "select [UserId],[Name],[Avatar],[Account],[Email],[Phone],[Address] from [user]";
            return this._connection.Query<User>(query, null, transaction).ToList();
        }
        public User GetUserByUserName(string userName, IDbTransaction transaction = null)
        {
            string query = "select * from [user] where Account = @userName or Phone = @userName";
            return this._connection.Query<User>(query, new { userName }, transaction).FirstOrDefault();
        }
        public User GetUserById(string UserId, IDbTransaction transaction = null)
        {
            string query = "select [UserId],[Name], [Avatar], [Account],[Email], [Phone], [Address], [Gender] [CreateTime], [Enable] from [user] where UserId = @UserId";
            return this._connection.Query<User>(query, new { UserId }, transaction).FirstOrDefault();
        }
        
        public User GetUserByToken(string Token, IDbTransaction transaction = null)
        {
            string query = "select u.* from [user] u join [user_token] ut on u.UserId = ut.UserId where ut.Token = @Token";
            return this._connection.Query<User>(query, new { Token }, transaction).FirstOrDefault();
        }
        public UserToken GetTokenByUserId(string userId, IDbTransaction transaction = null)
        {
            string query = "select * from [user_token] where UserId = @userId";
            return this._connection.Query<UserToken>(query, new { userId }, transaction).FirstOrDefault();
        }
        public UserToken GetTokenByToken(string token, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from [user_token] where Token = @token";
            return this._connection.Query<UserToken>(query, new { token }, transaction).FirstOrDefault();
        }
        public bool UpdatePassword(string userId, string newPassword, IDbTransaction transaction = null)
        {
            string update = "UPDATE [user] SET Password = @newPassword WHERE UserId = @userId";
            int status = this._connection.Execute(update, new { newPassword = newPassword, userId = userId }, transaction);
            return status > 0;
        }
        public bool InsertToken(UserToken model)
        {
            string query = "INSERT INTO [dbo].[user_token] ([UserTokenId],[UserId],[Token],[CreateTime],[FireBaseToken]) VALUES (@UserTokenId,@UserId,@Token,@CreateTime,@FireBaseToken)";
            return this._connection.Execute(query, model) > 0;
        }
        public bool UpdateConnectionId(UserToken model)
        {
            string query = "update [user_token] set ConnectionId=@ConnectionId where UserTokenId=@UserTokenId";
            return this._connection.Execute(query, model) > 0;
        }
        public bool DeleteToken(string UserId)
        {
            string query = "delete from [user_token] where UserId = @UserId";
            return this._connection.Execute(query, new { UserId }) > 0;
        }
        public bool InsertNewUser(User user, IDbTransaction transaction = null)
        {
            string query = "insert into [user] ([UserId],[Name],[Account],[Email],[Phone],[Password],[Enable], [SearchName]) values (@UserId, @Name, @Account, @Email, @Phone, @Password,@Enable, @SearchName)";
            var status = this._connection.Execute(query, user, transaction);
            return status > 0;
        }
        public string CheckDuplicateUser(string Keyword, IDbTransaction transaction = null)
        {
            string query = "select * from [user] where Phone = @Keyword or Account = @Keyword";
            return this._connection.Query<string>(query, new { Keyword }, transaction).FirstOrDefault();
        }
        public bool CheckPhoneExist(string phone, IDbTransaction transaction = null)
        {
            string query = "select count(*) from [user] where Phone = @phone and Phone <> '' ";
            int count = this._connection.Query<int>(query, new { phone }, transaction).FirstOrDefault();
            return count > 0;
        }
        public bool CheckAccountExist(string account, IDbTransaction transaction = null)
        {
            string query = "select count(*) from [user] where Account = @account and Account <> ''";
            int count = this._connection.Query<int>(query, new { account }, transaction).FirstOrDefault();
            return count > 0;
        }
        public string CheckDuplicateEmail(string Email)
        {
            string query = "select Name from [user] where Email = @Email";
            return this._connection.Query<string>(query, new { Email }).FirstOrDefault();
        }
        public string CheckDuplicatePhone(string Phone)
        {
            string query = $"select Name from [user] where Phone = '{Phone}'";
            return this._connection.Query<string>(query, new { Phone }).FirstOrDefault();
        }
        public bool UpdatePhone(string NewPhone, string UserId, IDbTransaction transaction = null)
        {
            string query = "update [user] set Phone = @NewPhone where UserId = @UserId";
            return this._connection.Execute(query, new { NewPhone, UserId }, transaction) > 0;
        }
        public List<UserToken> GetListUserToken(string userId, IDbTransaction transaction = null)
        {
            string query = "select * from [user_token] where UserId=@userId";
            return this._connection.Query<UserToken>(query, new { userId }, transaction).ToList();
        }

        public List<object> GetListSearchUser(string keyword)
        {
            keyword = keyword.Trim();
            keyword = "%" + keyword.Replace(" ", "%") + "%";
            string query = "select top 10 * from [user] where Account like @keyword or Phone like @keyword";
            return this._connection.Query<object>(query, new { keyword }).ToList();
        }

        public void RemoveUserToken(string token, IDbTransaction transaction = null)
        {
            string query = "update [user_token] set Token=NULL where Token=@token";
            int status = this._connection.Execute(query, new { token = token }, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        public UserToken GetUserTokenByUserId(string userId, IDbTransaction transaction = null)
        {
            string query = "select * from [user_token] where [UserId] = @userId";
            return this._connection.Query<UserToken>(query, new { userId }, transaction).FirstOrDefault();
        }
        public bool UpdateUserToken(UserToken userToken, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[user_token] SET [Token] = @Token, [CreateTime] = @CreateTime WHERE [UserId] = @UserId";
            int status = this._connection.Execute(query, userToken, transaction);
            return status > 0;
        }
        public bool InsertUserToken(UserToken userToken, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[user_token] ([UserTokenId],[UserId],[Token],[CreateTime]) VALUES (@UserTokenId,@UserId,@Token,@CreateTime)";
            int status = this._connection.Execute(query, userToken, transaction);
            return status > 0;
        }
        public object GetInforUser(string userId, IDbTransaction transaction = null)
        {
            string query = "select * from [user] where UserId = @userId";
            return this._connection.Query<object>(query, new { userId }, transaction).FirstOrDefault();
        }
        public bool UpdateInforUser(User model, IDbTransaction transaction = null)
        {
            string update = "UPDATE [dbo].[user] SET [Name] = @Name, [SearchName] = @SearchName, [Avatar] = @Avatar," +
                            "[Phone] = @Phone,[Address] = @Address WHERE [UserId] = @UserId";
            int status = this._connection.Execute(update, model, transaction);
            return status > 0;
        }
    }
}