using DentalClinic.Filters;
using DentalClinic.Models;
using DentalClinic.Providers;
using DentalClinic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace DentalClinic.Areas.Admin.ApiControllers
{
    public class AdminHomeController : ApiAdminBaseController
    {
        public JsonResult ChangePassword(UserAdminChangePasswordModel model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                        if (userAdmin == null) return Unauthorized();

                        UserAdminService userAdminService = new UserAdminService(connect);

                        model.OldPassword = SecurityProvider.EncodePassword(userAdmin.UserAdminId, model.OldPassword);
                        if (userAdmin.Password != model.OldPassword) throw new Exception("Mật khẩu cũ không chính xác");

                        model.NewPassword = SecurityProvider.EncodePassword(userAdmin.UserAdminId, model.NewPassword);

                        if (!userAdminService.UpdateUserAdminPassword(userAdmin.UserAdminId, model.NewPassword, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        transaction.Commit();
                        return Success();
                    }
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        [ApiAdminTokenRequire]
        [Permission("ADMIN")]
        public JsonResult GetTotalUserActive()
        {
            try
            {
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();
                ReportService reportService = new ReportService();
                return Success(reportService.GetTotalUser());
            }
            catch (Exception ex)
            {
                return Error();
            }
        }

        [HttpGet]
        [ApiAdminTokenRequire]
        [Permission("ADMIN")]
        public JsonResult GetTotalAppointment()
        {
            try
            {
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();
                ReportService reportService = new ReportService();
                return Success(reportService.GetTotalAppointment(UserAppointment.EnumStatus.PENDING));
            }
            catch (Exception ex)
            {
                return Error();
            }
        }
        [HttpGet]
        [ApiAdminTokenRequire]
        [Permission("ADMIN")]
        public JsonResult GetListReportSystemRevenueMonthly(int month, int year)
        {
            try
            {
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();
                ReportService reportService = new ReportService();
                return Success(reportService.GetListReportSystemRevenueMonthly(month, year));
            }
            catch (Exception ex)
            {
                return Error();
                throw;
            }
        }
        [HttpGet]
        [ApiAdminTokenRequire]
        [Permission("ADMIN")]
        public JsonResult GetListReportSystemRevenueYearly(int year)
        {
            try
            {
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();
                ReportService reportService = new ReportService();
                return Success(reportService.GetListReportSystemRevenueYearly(year));
            }
            catch (Exception ex)
            {
                return Error();
                throw;
            }
        }
        [HttpGet]
        [ApiAdminTokenRequire]
        [Permission("ADMIN")]
        public JsonResult GetReportSystemRevenueThisMonth()
        {
            try
            {
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();
                ReportService reportSystemRevenueService = new ReportService();
                return Success(reportSystemRevenueService.GetListReportSystemRevenueThisMonth());
            }
            catch (Exception ex)
            {
                return Error();
            }
        }

    }
}
