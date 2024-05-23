using Dapper;
using DentalClinic.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DentalClinic.Services
{
    public class AdminDoctorService : BaseService
    {
        public AdminDoctorService() : base() { }

        public AdminDoctorService(IDbConnection db) : base(db) { }
        public DoctorListViewModel GetListDoctor(int page, string keyword, IDbTransaction transaction = null)
        {
            DoctorListViewModel doctorListViewModel = new DoctorListViewModel();
            doctorListViewModel.TotalPage = 0;

            string querySelect = "select * ";
            string queryCount = "select count(*) as Total ";
            string query = " from [doctor] ";

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(' ', '%') + "%";
                query += " where SearchName like @keyword";
            }

            int totalRow = _connection.Query<int>(queryCount + query, new { keyword = keyword }, transaction).FirstOrDefault();
            if (totalRow > 0)
            {
                doctorListViewModel.TotalPage = (int)Math.Ceiling((decimal)totalRow / Constant.ADMIN_PAGE_SIZE);
            }

            int skip = (page - 1) * Constant.ADMIN_PAGE_SIZE;
            query += " order by CreateTime desc offset " + skip + " rows fetch next " + Constant.ADMIN_PAGE_SIZE + " rows only";
            doctorListViewModel.ListDoctor = this._connection.Query<Doctor>(querySelect + query, new { keyword = keyword }, transaction).ToList();
            return doctorListViewModel;
        }
        public Doctor GetDoctorById(string doctorId, IDbTransaction transaction = null)
        {
            string query = "select * from doctor where DoctorId=@doctorId ";
            return _connection.Query<Doctor>(query, new { doctorId }, transaction).FirstOrDefault();
        }
        public bool InsertDoctor(Doctor doctor, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[doctor] ([DoctorId],[Name],[SearchName],[Description],[Avatar],[Qualification],[Enable],[CreateTime]) VALUES ( @DoctorId, @Name, @SearchName, @Description, @Avatar, @Qualification, @Enable, @CreateTime)";
            return _connection.Execute(query, doctor, transaction) > 0;
        }
        public bool UpdateDoctor(Doctor doctor, IDbTransaction transaction = null)
        {
            string query = "UPDATE [doctor] SET [Name]=@Name, [SearchName]=@SearchName, [Description]=@Description,[Avatar]=@Avatar,[Qualification]=@Qualification,[Enable]=@Enable,[CreateTime]=@CreateTime  WHERE DoctorId=@DoctorId";
            return _connection.Execute(query, doctor, transaction) > 0; ;
        }
        public bool DeleteDoctor(string doctorId, IDbTransaction transaction = null)
        {
            string query = "DELETE FROM doctor where DoctorId=@doctorId";
            return _connection.Execute(query, new { doctorId }, transaction) > 0;
        }
        public List<Doctor> GetListTopDoctor(IDbTransaction transaction = null)
        {
            string query = "select TOP(10) DoctorId, Name, Thumbnail,Description,Avatar,Qualification from Doctor Order by CreateTime desc";
            return this._connection.Query<Doctor>(query, new { enable = true }, transaction).ToList();
        }
    }
}