using Dapper;
using DentalClinic.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DentalClinic.Services
{
    public class ReportService : BaseService
    {
        public ReportService() : base() { }
        public ReportService(IDbConnection db) : base(db) { }

        public int GetTotalUser(IDbTransaction transaction = null)
        {
            string query = "select count(*) as TotalUser from [user] where Enable = 1";
            return this._connection.Query<int>(query, null, transaction).FirstOrDefault();
        }
        public int GetTotalAppointment(string status, IDbTransaction transaction = null)
        {
            string query = "select count(*) as TotalAppointment from [user_appointment] where Status = @status";
            return this._connection.Query<int>(query, new { status = status }, transaction).FirstOrDefault();
        }
        public List<ReportDaily> GetListReportSystemRevenueMonthly(int month, int year, IDbTransaction transaction = null)
        {
            string query = "select * from report_daily where Month = @month and Year = @year order by Day";
            return this._connection.Query<ReportDaily>(query, new { month, year }, transaction).ToList();
        }
        public List<ReportMonthly> GetListReportSystemRevenueYearly(int year, IDbTransaction transaction = null)
        {
            string query = "select * from report_monthly where Year = @year order by Month";
            return this._connection.Query<ReportMonthly>(query, new { year }, transaction).ToList();
        }
        public SystemWallet GetCashOfSystem(string systemWalletId, IDbTransaction transaction = null)
        {
            string query = "select * from system_wallet where SystemWalletId = @systemWalletId";
            return this._connection.Query<SystemWallet>(query, new { systemWalletId }, transaction).FirstOrDefault();
        }
        public object GetListReportSystemRevenueThisMonth(IDbTransaction transaction = null)
        {
            string query = "select * from report_daily where Month = MONTH(GETDATE()) and Year = YEAR(GETDATE()) order by Year, Month, Day";
            string queryTotalAmount = "select TotalPrice from report_monthly where Month = MONTH(GETDATE()) and Year = YEAR(GETDATE())";
            decimal? TotalPrice = this._connection.Query<decimal?>(queryTotalAmount, null, transaction).FirstOrDefault();
            List<ReportDaily> ListReportSystemRevenueDaily = this._connection.Query<ReportDaily>(query, null, transaction).ToList();
            return new { TotalPrice, ListReportSystemRevenueDaily };
        }
    }
}