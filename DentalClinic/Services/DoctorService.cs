using Dapper;
using DentalClinic.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DentalClinic.Services
{
    public class DoctorService : BaseService
    {
        public DoctorService() : base() { }
        public DoctorService(IDbConnection db) : base(db) { }
        public List<Doctor> GetListDoctorForHomePage(IDbTransaction transaction = null)
        {
            string query = "select TOP(10) * from doctor Where Enable = 1 order by CreateTime DESC";
            return this._connection.Query<Doctor>(query, new { enable = true }, transaction).ToList();
        }
        public List<Doctor> GetListDoctor()
        {
            string query = "select * from doctor where Enable = 1";
            return this._connection.Query<Doctor>(query).ToList();
        }

        public Doctor GetDoctorById(string DoctorId, IDbTransaction transaction = null)
        {
            string query = "select * from doctor where DoctorId = @DoctorId";
            return this._connection.Query<Doctor>(query, new { DoctorId }, transaction).FirstOrDefault();
        }
        public DoctorListViewModel GetListDoctorForListDoctor(int page, IDbTransaction transaction = null)
        {
            string queryselect = "select * ";
            string querycount = "select count(*) as Total ";
            string query = " from doctor where Enable=@enable";

            DoctorListViewModel doctorListViewModel = new DoctorListViewModel();
            doctorListViewModel.ListDoctor = new List<Doctor>();
            doctorListViewModel.TotalPage = 0;

            int totalRows = this._connection.Query<int>(querycount + query, new { enable = true }, transaction).FirstOrDefault();
            if (totalRows > 0)
            {
                doctorListViewModel.TotalPage = (int)Math.Ceiling((decimal)totalRows / Constant.PAGE_NEWS_SIZE);
            }
            if (page == 0)
            {
                page = 1;
            }
            int skip = (page - 1) * Constant.PAGE_NEWS_SIZE;
            query += " order by CreateTime desc";
            query += " offset " + skip + " rows fetch next " + Constant.PAGE_NEWS_SIZE + " rows only";
            doctorListViewModel.ListDoctor = this._connection.Query<Doctor>(queryselect + query, new { enable = true }, transaction).ToList();
            return doctorListViewModel;
        }
        public Doctor GetDoctorDetail(string doctorId, IDbTransaction transaction = null)
        {
            string query = "select * from [doctor] where Enable=@enable and DoctorId=@doctorId";
            return this._connection.Query<Doctor>(query, new { enable = true, doctorId }, transaction).FirstOrDefault();
        }
        public List<Doctor> GetListRelatedDoctor(IDbTransaction transaction = null)
        {
            string query = "select TOP(10) * from doctor where Enable=@enable Order by CreateTime desc";
            return this._connection.Query<Doctor>(query, new { enable = true }, transaction).ToList();
        }
    }
}