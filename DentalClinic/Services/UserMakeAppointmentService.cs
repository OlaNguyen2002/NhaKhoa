using Dapper;
using DentalClinic.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DentalClinic.Services
{
    public class UserMakeAppointmentService : BaseService
    {
        public UserMakeAppointmentService() : base() { }
        public UserMakeAppointmentService(IDbConnection db) : base(db) { }
        public bool CreateUserAppointment(UserAppointment model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[user_appointment]([UserAppointmentId],[UserId], [AppointmentCode], [Name],[Phone],[Email],[Address],[DoctorId],[Hour],[Minute],[Day],[Month],[Year],[Status],[CreateTime],[TotalExpectTime],[TotalAmount]) " +
                "VALUES (@UserAppointmentId, @UserId, @AppointmentCode,  @Name, @Phone, @Email, @Address, @DoctorId, @Hour, @Minute,@Day,  @Month, @Year,@Status, @CreateTime, @TotalExpectTime,@TotalAmount )";
            int status = this._connection.Execute(query, model, transaction);
            return status > 0;
        }
        public bool CreateUserAppointmentService(UserAppointmentService model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[user_appointment_service]([UserAppointmentServiceId],[UserAppointmentId],[ServiceId],[ExpectTime]) " +
                "VALUES (@UserAppointmentServiceId, @UserAppointmentId, @ServiceId, @ExpectTime)";
            int status = this._connection.Execute(query, model, transaction);
            return status > 0;
        }
        public UserAppointment GetUserAppointmentById(string userAppointmentId, IDbTransaction transaction = null)
        {
            string query = "select * from [dbo].[user_appointment] where UserAppointmentId=@userAppointmentId";
            return this._connection.Query<UserAppointment>(query, new { userAppointmentId }, transaction).FirstOrDefault();
        }
        public ListUserAppointmentView GetListAppointmentOfUser(int page, string keyword, string userId, IDbTransaction transaction = null)
        {
            ListUserAppointmentView listUserAppointmentView = new ListUserAppointmentView();
            string queryCount = "select count(*) as Total ";
            string querySelect = "select ua.*, d.Name as NameDoctor ";
            string query = " from [user_appointment] ua join [doctor] d on ua.DoctorId = d.DoctorId where ua.UserId = @userId ";
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(' ', '%') + "%";
                query += " and ua.Name like @keyword ";
            }
            int TotalPage = 0;
            int totalRow = this._connection.Query<int>(queryCount + query, new { page = page, keyword = keyword, userId = userId }, transaction).FirstOrDefault();
            if (totalRow > 0)
            {
                TotalPage = (int)Math.Ceiling((decimal)totalRow / Constant.PAGE_SIZE);
            }
            int skip = (page - 1) * Constant.PAGE_SIZE;
            query += " order by CreateTime desc offset " + skip + " rows fetch next " + Constant.PAGE_SIZE + " rows only ";
            listUserAppointmentView.ListUserAppointmentInfor = this._connection.Query<UserAppointmentInfor>(querySelect + query, new { page = page, keyword = keyword, userId = userId }, transaction).ToList();
            listUserAppointmentView.TotalPage = TotalPage;
            return listUserAppointmentView;
        }
        public UserAppointment GetAppointmentByAppointmentCode(string appointmentCode, IDbTransaction transaction = null)
        {
            string query = "select * from [dbo].[user_appointment] where AppointmentCode=@appointmentCode";
            return this._connection.Query<UserAppointment>(query, new { appointmentCode }, transaction).FirstOrDefault();
        }
        public List<UserAppointmentServiceUpdate> GetListUserAppointmentServiceUpdateByUserAppointmentId(string userAppointmentId, IDbTransaction transaction = null)
        {
            string query = "Select uas.*,s.Name As NameService,s.ExpectTime, s.Price As PriceService from [user_appointment_service] uas left join [service_dental] s on uas.ServiceId=s.ServiceId where UserAppointmentId =@userAppointmentId";
            return this._connection.Query<UserAppointmentServiceUpdate>(query, new { userAppointmentId }, transaction).ToList();
        }
        public List<UserAppointment> GetListUserAppointmentByDoctorId(string doctorId,int day,int month,int year, IDbTransaction transaction = null)
        {
            string query = "select ua.* from user_appointment ua left join doctor d on ua.DoctorId = d.DoctorId where ua.DoctorId=@doctorId and ua.Day=@day and ua.Month = @month and ua.Year = @year";
            return this._connection.Query<UserAppointment>(query, new { doctorId,day,month,year }, transaction).ToList();
        }
        public bool UpdateUserAppointmentStatus(string userAppointmentId, string status, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[user_appointment] SET [Status] = @status WHERE UserAppointmentId = @userAppointmentId";
            int _status = this._connection.Execute(query, new { userAppointmentId, status }, transaction);
            return _status > 0;
        }
        public List<UserAppointmentServiceUpdate> GetListAppointmentDetailByAppointmentId(string userAppointmentId, IDbTransaction transaction = null)
        {
            string query = "Select * from user_appointment where UserAppointmentId =@userAppointmentId";
            return this._connection.Query<UserAppointmentServiceUpdate>(query, new { userAppointmentId }, transaction).ToList();
        }
    }
}