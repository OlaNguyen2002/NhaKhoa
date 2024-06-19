using DentalClinic.Models;
using DentalClinic.Providers;
using DentalClinic.Services;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Web.Http;
using System.Web.Http.Results;

namespace DentalClinic.Areas.Admin.ApiControllers
{
    public class AdminManageAppointmentController : ApiAdminBaseController
    {
        [HttpGet]
        public JsonResult GetListUserAppointment()
        {
            try
            {
                AdminManageAppointmentService adminManageAppointmentService = new AdminManageAppointmentService();
                return Success(adminManageAppointmentService.GetListUserAppointment());
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }


        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult GetListAppointmentOfUser(string UserId, int PageIndex, string AppointmentCode = "", long? CreateStart = null, long? CreateEnd = null)
        {
            try
            {
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();

                AdminManageAppointmentService adminManageAppointmentService = new AdminManageAppointmentService();
                ListUserAppointmentView listUserAppointmentView = adminManageAppointmentService.GetListAppointmentOfUser(UserId, PageIndex, AppointmentCode, CreateStart, CreateEnd);
                for (int i = 0; i < listUserAppointmentView.ListUserAppointmentInfor.Count; i++)
                {
                    var item = listUserAppointmentView.ListUserAppointmentInfor[i];
                    List<ServiceDental> lsServiceDental = adminManageAppointmentService.GetServiceByUserAppointmentId(item.UserAppointmentId);
                    decimal totalPriceService = 0;
                    foreach (var _item in lsServiceDental)
                    {
                        totalPriceService += _item.Price;
                    }
                    item.lsServiceDental = new List<ServiceDental>();
                    item.lsServiceDental.AddRange(lsServiceDental);
                    item.TotalAmount = totalPriceService;
                }
                return Success(listUserAppointmentView);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    
        [HttpPost]
        public JsonResult AddUserAppointment(User model)
        {
            try
            {
                using(var connection = BaseService.Connect())
                {
                    connection.Open();
                    using(var transaction = connection.BeginTransaction())
                    {
                        AdminManageAppointmentService adminManageAppointmentService = new AdminManageAppointmentService(connection);
                        User user = new User();
                        user.UserId = Guid.NewGuid().ToString();
                        user.Name = model.Name;
                        user.SearchName = model.SearchName.Trim();
                        user.Phone = model.Phone;
                        user.Email = model.Email.Trim();
                        user.Address = model.Address;
                        user.Enable = true;
                        user.CreateTime = HelperProvider.GetSeconds();
                        if (!adminManageAppointmentService.AddUser(user, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
                        transaction.Commit();
                        return Success(user);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [ApiAdminTokenRequire]
        public JsonResult AddAppointment(UserAppointmentRequest model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserService userService = new UserService(connect);
                        DoctorService doctorService = new DoctorService(connect);
                        ServiceService serviceService = new ServiceService(connect);
                        UserMakeAppointmentService userMakeAppointmentService = new UserMakeAppointmentService(connect);  
                        
                        User user = userService.GetUserById(model.UserId, transaction);
                        if (user == null) throw new Exception("Không tìm thấy người dùng");

                        Doctor doctor = doctorService.GetDoctorById(model.DoctorId, transaction);
                        if (doctor == null) return Error("Bạn chưa chọn bác sĩ.");

                        long now = HelperProvider.GetSeconds();
                        long timeOrder = HelperProvider.GetSeconds(new DateTime(model.Year, model.Month, model.Day, model.Hour, model.Minute, 0, 0));
                        if (timeOrder <= now) return Error("Bạn phải đặt sau thời điểm hiện tại");

                        if (model.Hour <= 0) return Error("Bạn chưa chọn giờ hẹn.");

                        UserAppointment userAppointment = new UserAppointment();
                        userAppointment.UserAppointmentId = Guid.NewGuid().ToString();
                        userAppointment.UserId = user.UserId;
                        userAppointment.AppointmentCode = HelperProvider.MakeCode();
                        userAppointment.Name = user.Name;
                        userAppointment.Phone = user.Phone;
                        userAppointment.Email = user.Email;
                        userAppointment.Address = user.Address;
                        userAppointment.DoctorId = doctor.DoctorId;
                        userAppointment.Hour = model.Hour;
                        userAppointment.Minute = model.Minute;
                        userAppointment.Day = model.Day;
                        userAppointment.Month = model.Month;
                        userAppointment.Year = model.Year;
                        userAppointment.Status = UserAppointment.EnumStatus.PENDING;
                        userAppointment.CreateTime = HelperProvider.GetSeconds();

                        int totalExpectTime = 0;
                        decimal totalPrice = 0;
                        if (model.ListServiceId.Count <= 0) return Error("Bạn chưa chọn dịch vụ nào.");
                        for (int i = 0; i < model.ListServiceId.Count; i++)
                        {
                            string ServiceId = model.ListServiceId[i].ServiceId;
                            ServiceDental serviceDental = serviceService.GetServiceById(ServiceId, transaction);

                            UserAppointmentService userAppointmentService = new UserAppointmentService();
                            userAppointmentService.UserAppointmentServiceId = Guid.NewGuid().ToString();
                            userAppointmentService.UserAppointmentId = userAppointment.UserAppointmentId;
                            userAppointmentService.ServiceId = serviceDental.ServiceId;
                            userAppointmentService.ExpectTime = serviceDental.ExpectTime;
                            totalExpectTime += serviceDental.ExpectTime;
                            totalPrice += serviceDental.Price;
                            if (!userMakeAppointmentService.CreateUserAppointmentService(userAppointmentService, transaction)) throw new Exception();
                        }
                        userAppointment.TotalExpectTime = totalExpectTime;
                        userAppointment.TotalAmount = totalPrice;
                        // lấy ra danh sách đơn của bác sĩ ngày đơn đặt mới
                        List<UserAppointment> lsuserAppointmentsByDoctor = userMakeAppointmentService.GetListUserAppointmentByDoctorId(model.DoctorId, model.Day, model.Month, model.Year, transaction);
                        List<TimeDoctor> lstimeDoctors = new List<TimeDoctor>();
                        for (int i = 0; i < lsuserAppointmentsByDoctor.Count; i++)
                        {
                            TimeDoctor timeDoctor = new TimeDoctor();
                            timeDoctor.MinTime = lsuserAppointmentsByDoctor[i].Hour * 60 + lsuserAppointmentsByDoctor[i].Minute;
                            timeDoctor.MaxTime = timeDoctor.MinTime + lsuserAppointmentsByDoctor[i].TotalExpectTime;
                            lstimeDoctors.Add(timeDoctor);
                        }
                        int minTime = model.Hour * 60 + model.Minute;
                        int maxTime = minTime + userAppointment.TotalExpectTime;
                        bool checkOrder = UserProvider.CheckTimeOrder(minTime, maxTime, lstimeDoctors, connect, transaction);
                        if (checkOrder == false)
                        {
                            throw new Exception("Bác sĩ bận trong thời gian này, vui lòng chọn bác sĩ hoặc thời gian khác");
                        }
                        if (!userMakeAppointmentService.CreateUserAppointment(userAppointment, transaction)) throw new Exception();
                        transaction.Commit();
                        return Success(new { AppointmentCode = userAppointment.AppointmentCode });
                    }
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [ApiAdminTokenRequire]
        public JsonResult UpdateAppointment(UserAppointmentModel model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {

                        UserService userService = new UserService(connect);
                        DoctorService doctorService = new DoctorService(connect);
                        ServiceService serviceService = new ServiceService(connect);
                        UserMakeAppointmentService userMakeAppointmentService = new UserMakeAppointmentService(connect);

                        UserAppointment userAppointment = userMakeAppointmentService.GetUserAppointmentById(model.UserAppointmentId, transaction);
                        if (userAppointment == null) throw new Exception("Không tồn tại lịch hẹn này");
                        if (userAppointment.Status == UserAppointment.EnumStatus.DONE) throw new Exception("Hóa đơn đã hoàn thành không thể sửa!");


                        Doctor doctor = doctorService.GetDoctorById(model.DoctorId, transaction);
                        if (doctor == null) return Error("Bạn chưa chọn bác sĩ.");

                        long now = HelperProvider.GetSeconds();

                        if(model.Hour != userAppointment.Hour && model.Minute != userAppointment.Minute && model.Day != userAppointment.Day && model.Month != userAppointment.Month && model.Year != userAppointment.Year)
                        {
                            long timeOrder = HelperProvider.GetSeconds(new DateTime(model.Year, model.Month, model.Day, model.Hour, model.Minute, 0, 0));
                            if (timeOrder <= now) return Error("Bạn phải đặt sau thời điểm hiện tại");
                        }

                        

                        if (model.Hour <= 0) return Error("Bạn chưa chọn giờ hẹn.");

                        

                        userAppointment.DoctorId = doctor.DoctorId;
                        userAppointment.Hour = model.Hour;
                        userAppointment.Minute = model.Minute;
                        userAppointment.Day = model.Day;
                        userAppointment.Month = model.Month;
                        userAppointment.Year = model.Year;

                        

                        int totalExpectTime = 0;
                        decimal totalPrice = 0;
                        if (model.ListServiceId.Count <= 0) return Error("Bạn chưa chọn dịch vụ nào.");

                        userMakeAppointmentService.DeleteUserAppointmentService(userAppointment.UserAppointmentId, transaction);

                        for (int i = 0; i < model.ListServiceId.Count; i++)
                        {
                            string ServiceId = model.ListServiceId[i].ServiceId;
                            ServiceDental serviceDental = serviceService.GetServiceById(ServiceId, transaction);

                            UserAppointmentService userAppointmentService = new UserAppointmentService();
                            userAppointmentService.UserAppointmentServiceId = Guid.NewGuid().ToString();
                            userAppointmentService.UserAppointmentId = userAppointment.UserAppointmentId;
                            userAppointmentService.ServiceId = serviceDental.ServiceId;
                            userAppointmentService.ExpectTime = serviceDental.ExpectTime;
                            totalExpectTime += serviceDental.ExpectTime;
                            totalPrice += serviceDental.Price;
                            if (!userMakeAppointmentService.CreateUserAppointmentService(userAppointmentService, transaction)) throw new Exception();
                        }
                        userAppointment.TotalExpectTime = totalExpectTime;
                        userAppointment.TotalAmount = totalPrice;
                        userMakeAppointmentService.UpdateHourDateUserAppointment(model.UserAppointmentId, transaction);
                        // lấy ra danh sách đơn của bác sĩ ngày đơn đặt mới
                        List<UserAppointment> lsuserAppointmentsByDoctor = userMakeAppointmentService.GetListUserAppointmentByDoctorId(model.DoctorId, model.Day, model.Month, model.Year, transaction);
                        List<TimeDoctor> lstimeDoctors = new List<TimeDoctor>();
                        for (int i = 0; i < lsuserAppointmentsByDoctor.Count; i++)
                        {
                            TimeDoctor timeDoctor = new TimeDoctor();
                            timeDoctor.MinTime = lsuserAppointmentsByDoctor[i].Hour * 60 + lsuserAppointmentsByDoctor[i].Minute;
                            timeDoctor.MaxTime = timeDoctor.MinTime + lsuserAppointmentsByDoctor[i].TotalExpectTime;
                            lstimeDoctors.Add(timeDoctor);
                        }
                        int minTime = model.Hour * 60 + model.Minute;
                        int maxTime = minTime + userAppointment.TotalExpectTime;
                        bool checkOrder = UserProvider.CheckTimeOrder(minTime, maxTime, lstimeDoctors, connect, transaction);
                        if (checkOrder == false)
                        {
                            throw new Exception("Bác sĩ bận trong thời gian này, vui lòng chọn bác sĩ hoặc thời gian khác");
                        }
                        userMakeAppointmentService.UpdateUserAppointment(userAppointment, transaction);

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
        public JsonResult GetListAllAppointment(int PageIndex, string AppointmentCode = "", string CreateStart = "", string CreateEnd = "", string Status ="")
        {
            try
            {
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();

                AdminManageAppointmentService adminManageAppointmentService = new AdminManageAppointmentService();
                ListUserAppointmentView listUserAppointmentView = adminManageAppointmentService.GetListAllAppointment(PageIndex, AppointmentCode, CreateStart, CreateEnd, Status);
                for (int i = 0; i < listUserAppointmentView.ListUserAppointmentInfor.Count; i++)
                {
                    var item = listUserAppointmentView.ListUserAppointmentInfor[i];
                    List<ServiceDental> lsServiceDental = adminManageAppointmentService.GetServiceByUserAppointmentId(item.UserAppointmentId);
                    decimal totalPriceService = 0;
                    foreach (var _item in lsServiceDental)
                    {
                        totalPriceService += _item.Price;
                    }
                    item.lsServiceDental = new List<ServiceDental>();
                    item.lsServiceDental.AddRange(lsServiceDental);
                    item.TotalAmount = totalPriceService;
                }
                return Success(listUserAppointmentView);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }


        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult ConfirmAppointment(string userAppointmentId)
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

                        UserMakeAppointmentService userMakeAppointmentService = new UserMakeAppointmentService(connect);
                        AppointmentStatusService appointmentStatusService = new AppointmentStatusService(connect);
                        // Lấy ra user appointment
                        UserAppointment userAppointment = userMakeAppointmentService.GetUserAppointmentById(userAppointmentId, transaction);
                        if (userAppointment == null) throw new Exception("Không tìm thấy lịch hẹn !");
                        if (userAppointment.Status != UserAppointment.EnumStatus.PENDING) throw new Exception("Trạng thái lịch hẹn không hợp lệ !");

                        // Set lại trạng thái appointment
                        if (!userMakeAppointmentService.UpdateUserAppointmentStatus(userAppointmentId, UserAppointment.EnumStatus.RECEIVE, transaction)) throw new Exception();

                        // Lưu lịch sử appointment_status
                        AppointmentStatus appointmentStatus = new AppointmentStatus();
                        appointmentStatus.AppointmentStatusId = Guid.NewGuid().ToString();
                        appointmentStatus.UserAppointmentId = userAppointmentId;
                        appointmentStatus.Status = UserAppointment.EnumStatus.RECEIVE;
                        appointmentStatus.CreateTime = HelperProvider.GetSeconds();
                        if (!appointmentStatusService.CreateAppointmentStatus(appointmentStatus, transaction)) throw new Exception();

                        //Email
                        if (!string.IsNullOrEmpty(userAppointment.Email))
                            SMSProvider.SendOTPViaEmail(userAppointment.Email, "", "THÔNG BÁO XÁC NHẬN"," Phòng khám nha khoa Hữu Tuấn đã tiếp nhận lịch hẹn [" + userAppointment.AppointmentCode + "] của bạn. Hãy chú ý để đến phòng khám đúng hẹn. Xin trân trọng cảm ơn!");

                        // Thông báo cho người dùng
                        Notification notification = new Notification();
                        notification.UserId = userAppointment.UserId;
                        notification.Title = "Thông báo";
                        notification.Message = "Lịch hẹn: " + userAppointment.AppointmentCode + " của bạn đã được tiếp nhận bởi phòng khám.";
                        notification.IsRead = false;
                        notification.CreateTime = HelperProvider.GetSeconds();
                        if (!NotificationProvider.CreateNotification(notification, connect, transaction)) throw new Exception();

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
        public JsonResult SystemCancelAppointment(string userAppointmentId)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserService userService = new UserService(connect);
                        UserMakeAppointmentService userMakeAppointmentService = new UserMakeAppointmentService(connect);
                        AppointmentStatusService appointmentStatusService = new AppointmentStatusService(connect);
                        UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                        if (userAdmin == null) return Unauthorized();

                        // Lấy ra
                        UserAppointment userAppointment = userMakeAppointmentService.GetUserAppointmentById(userAppointmentId, transaction);
                        if (userAppointment == null) throw new Exception("Không tìm thấy lịch hẹn !");
                        if (userAppointment.Status == UserAppointment.EnumStatus.DONE || userAppointment.Status == UserAppointment.EnumStatus.USER_CANCEL || userAppointment.Status == UserAppointment.EnumStatus.SYSTEM_CANCEL) throw new Exception("Trạng thái lịch hẹn không hợp lệ !");
                        // Set lại trạng thái 
                        if (!userMakeAppointmentService.UpdateUserAppointmentStatus(userAppointmentId, UserAppointment.EnumStatus.SYSTEM_CANCEL, transaction)) throw new Exception();

                        // Lưu lịch sử Order_status
                        AppointmentStatus appointmentStatus = new AppointmentStatus();
                        appointmentStatus.AppointmentStatusId = Guid.NewGuid().ToString();
                        appointmentStatus.UserAppointmentId = userAppointmentId;
                        appointmentStatus.Status = UserAppointment.EnumStatus.SYSTEM_CANCEL;
                        appointmentStatus.CreateTime = HelperProvider.GetSeconds();
                        if (!appointmentStatusService.CreateAppointmentStatus(appointmentStatus, transaction)) throw new Exception();

                        // Thông báo cho người dùng
                        Notification notification = new Notification();
                        notification.UserId = userAppointment.UserId;
                        notification.Title = "Thông báo";
                        notification.Message = "Lịch hẹn: " + userAppointment.AppointmentCode + " của bạn đã bị hủy bởi phòng khám.";
                        notification.IsRead = false;
                        notification.CreateTime = HelperProvider.GetSeconds();
                        if (!NotificationProvider.CreateNotification(notification, connect, transaction)) throw new Exception();

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
        public JsonResult ConfirmArrive(string userAppointmentId)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserService userService = new UserService(connect);
                        UserMakeAppointmentService userMakeAppointmentService = new UserMakeAppointmentService(connect);
                        AppointmentStatusService appointmentStatusService = new AppointmentStatusService(connect);
                        UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                        if (userAdmin == null) return Unauthorized();

                        // Lấy ra
                        UserAppointment userAppointment = userMakeAppointmentService.GetUserAppointmentById(userAppointmentId, transaction);
                        if (userAppointment == null) throw new Exception("Không tìm thấy lịch hẹn !");
                        if (userAppointment.Status == UserAppointment.EnumStatus.DONE || userAppointment.Status == UserAppointment.EnumStatus.USER_CANCEL || userAppointment.Status == UserAppointment.EnumStatus.SYSTEM_CANCEL) throw new Exception("Trạng thái lịch hẹn không hợp lệ !");
                        // Set lại trạng thái 
                        if (!userMakeAppointmentService.UpdateUserAppointmentStatus(userAppointmentId, UserAppointment.EnumStatus.CONFIRM_ARRIVE, transaction)) throw new Exception();

                        // Lưu lịch sử status
                        AppointmentStatus appointmentStatus = new AppointmentStatus();
                        appointmentStatus.AppointmentStatusId = Guid.NewGuid().ToString();
                        appointmentStatus.UserAppointmentId = userAppointmentId;
                        appointmentStatus.Status = UserAppointment.EnumStatus.CONFIRM_ARRIVE;
                        appointmentStatus.CreateTime = HelperProvider.GetSeconds();
                        if (!appointmentStatusService.CreateAppointmentStatus(appointmentStatus, transaction)) throw new Exception();

                        // Thông báo cho người dùng
                        Notification notification = new Notification();
                        notification.UserId = userAppointment.UserId;
                        notification.Title = "Thông báo";
                        notification.Message = "Phòng khám xác nhận bạn đã đến phòng khám.";
                        notification.IsRead = false;
                        notification.CreateTime = HelperProvider.GetSeconds();
                        if (!NotificationProvider.CreateNotification(notification, connect, transaction)) throw new Exception();

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
        public JsonResult DoneAppointment(string userAppointmentId)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserMakeAppointmentService userMakeAppointmentService = new UserMakeAppointmentService(connect);
                        AppointmentStatusService appointmentStatusService = new AppointmentStatusService(connect);
                        AdminSystemWalletService adminSystemWalletService = new AdminSystemWalletService(connect);
                        AdminSystemTransactionService adminSystemTransactionService = new AdminSystemTransactionService(connect);
                        AdminReportService adminReportService = new AdminReportService(connect);
                        UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                        if (userAdmin == null) return Unauthorized();

                        // Lấy ra
                        UserAppointment userAppointment = userMakeAppointmentService.GetUserAppointmentById(userAppointmentId, transaction);
                        if (userAppointment == null) throw new Exception("Không tìm thấy lịch hẹn !");
                        if (userAppointment.Status == UserAppointment.EnumStatus.DONE || userAppointment.Status == UserAppointment.EnumStatus.USER_CANCEL || userAppointment.Status == UserAppointment.EnumStatus.SYSTEM_CANCEL) throw new Exception("Trạng thái lịch hẹn không hợp lệ !");

                        // Set lại trạng thái 
                        if (!userMakeAppointmentService.UpdateUserAppointmentStatus(userAppointmentId, UserAppointment.EnumStatus.DONE, transaction)) throw new Exception();

                        DateTime now = DateTime.Now;
                        // Cộng tiền thu của hóa đơn vào trong ví khách sạn, và báo cáo
                        adminSystemWalletService.UpdateRevenueSystem(userAppointment.TotalAmount, transaction);

                        SystemTransaction systemTransaction = new SystemTransaction();
                        systemTransaction.SystemTransactionId = Guid.NewGuid().ToString();
                        systemTransaction.Amount = userAppointment.TotalAmount;
                        systemTransaction.Message = "Tiền thu từ hóa đơn của khách hàng " + userAppointment.Name + " đã khám xong.";
                        systemTransaction.CreateTime = HelperProvider.GetSeconds(now);
                        adminSystemTransactionService.InsertSystemTransaction(systemTransaction, transaction);

                        // Thêm vào bảng báo cáo
                        ReportDaily reportDaily = adminReportService.GetReportDailyByDayMonthYear(now.Day, now.Month, now.Year, transaction);
                        if (reportDaily == null)
                        {
                            reportDaily = new ReportDaily();
                            reportDaily.ReportDailyId = Guid.NewGuid().ToString();
                            reportDaily.TotalPrice = userAppointment.TotalAmount;
                            reportDaily.Day = now.Day;
                            reportDaily.Month = now.Month;
                            reportDaily.Year = now.Year;
                            adminReportService.InsertReportDaily(reportDaily, transaction);
                        }
                        else
                        {
                            adminReportService.UpdateTotalPriceByReportDailyId(userAppointment.TotalAmount, reportDaily.ReportDailyId, transaction);
                        }
                        ReportMonthly reportMonthly = adminReportService.GetReportMonthlyByMonthYear(now.Month, now.Year, transaction);
                        if (reportMonthly == null)
                        {
                            reportMonthly = new ReportMonthly();
                            reportMonthly.ReportMonthlyId = Guid.NewGuid().ToString();
                            reportMonthly.TotalPrice = userAppointment.TotalAmount;
                            reportMonthly.Month = now.Month;
                            reportMonthly.Year = now.Year;
                            adminReportService.InsertReportMonthly(reportMonthly, transaction);
                        }
                        else
                        {
                            adminReportService.UpdateTotalPriceByReportMonthlyId(userAppointment.TotalAmount, reportMonthly.ReportMonthlyId, transaction);
                        }
                        ReportYearly reportYearly = adminReportService.GetReportYearlyByYear(now.Year, transaction);
                        if (reportYearly == null)
                        {
                            reportYearly = new ReportYearly();
                            reportYearly.ReportYearlyId = Guid.NewGuid().ToString();
                            reportYearly.TotalPrice = userAppointment.TotalAmount;
                            reportYearly.Year = now.Year;
                            adminReportService.InsertReportYearly(reportYearly, transaction);
                        }
                        else
                        {
                            adminReportService.UpdateTotalPriceByReportYearlyId(userAppointment.TotalAmount, reportYearly.ReportYearlyId, transaction);
                        }


                        // Lưu lịch sử status
                        AppointmentStatus appointmentStatus = new AppointmentStatus();
                        appointmentStatus.AppointmentStatusId = Guid.NewGuid().ToString();
                        appointmentStatus.UserAppointmentId = userAppointmentId;
                        appointmentStatus.Status = UserAppointment.EnumStatus.DONE;
                        appointmentStatus.CreateTime = HelperProvider.GetSeconds();
                        if (!appointmentStatusService.CreateAppointmentStatus(appointmentStatus, transaction)) throw new Exception();

                        // Thông báo cho người dùng
                        Notification notification = new Notification();
                        notification.UserId = userAppointment.UserId;
                        notification.Title = "Thông báo";
                        notification.Message = "Lịch hẹn: " + userAppointment.AppointmentCode + " của bạn đã hoàn thành.";
                        notification.IsRead = false;
                        notification.CreateTime = HelperProvider.GetSeconds();
                        if (!NotificationProvider.CreateNotification(notification, connect, transaction)) throw new Exception();

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
        //[ApiAdminTokenRequire]
        public JsonResult GetAppointmentDetail(string userAppointmentId)
        {
            try
            {
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();
                UserMakeAppointmentService userMakeAppointmentService = new UserMakeAppointmentService();
                UserAppointment userAppointment = userMakeAppointmentService.GetUserAppointmentById(userAppointmentId);
                if (userAppointment == null) return Error("Lịch hẹn này không tồn tại!");
                List<UserAppointmentServiceUpdate> listUserAppointmentDetail = userMakeAppointmentService.GetListUserAppointmentServiceUpdateByUserAppointmentId(userAppointmentId);
                return Success(new { userAppointment, listUserAppointmentDetail });
            }
            catch (Exception ex)
            {
                return Error();
            }
        }

        private void BindingFormatForExcelOrder(ExcelWorksheet worksheet, ExportUserAppointment model)
        {
            worksheet.DefaultColWidth = 10;
            worksheet.Cells.Style.WrapText = true;

            worksheet.Cells["A1:D1"].Value = "NHA KHOA HỮU TUẤN";
            worksheet.Cells["A2:D2"].Value = "HÓA ĐƠN ĐẶT PHÒNG";


            worksheet.Cells["A4"].Value = "Họ và tên khách hàng: ";
            worksheet.Cells["A5"].Value = "Số điện thoại: ";
            worksheet.Cells["A6"].Value = "Email: ";
            worksheet.Cells["B4"].Value = model.UserAppointmentInfo.Name;
            worksheet.Cells["B5"].Value = model.UserAppointmentInfo.Phone;
            worksheet.Cells["B6"].Value = model.UserAppointmentInfo.Email;

            DateTime hour = HelperProvider.GetDateTime(model.UserAppointmentInfo.Hour);
            DateTime date = HelperProvider.GetDateTime(model.UserAppointmentInfo.Day);

            worksheet.Cells["D4"].Value = "Mã lịch hẹn: ";
            worksheet.Cells["D5"].Value = "Giờ hẹn : ";
            worksheet.Cells["D6"].Value = "Ngày hẹn: ";
            worksheet.Cells["E4"].Value = model.UserAppointmentInfo.AppointmentCode;
            worksheet.Cells["E5"].Value = hour;
            worksheet.Cells["E6"].Value = date;

            worksheet.Cells["A7"].Value = "STT: ";
            worksheet.Cells["B7"].Value = "Dịch vụ: ";
            worksheet.Cells["C7"].Value = "Giá dịch vụ: ";

            using (var range = worksheet.Cells["A7:C7"])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 221, 177));
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.Font.SetFromFont("Arial", 10);
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                range.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.WhiteSmoke);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.Font.Bold = true;
            }
            using (var range = worksheet.Cells["A8:C8"])
            {
                range.Style.Font.Bold = true;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            for (int i = 0; i < model.ListServiceDental.Count; i++)
            {
                worksheet.Cells["A" + (i + 8)].Value = i + 1;
            }
            for (int i = 0; i < model.ListServiceDental.Count; i++)
            {
                var item = model.ListServiceDental[i];
                worksheet.Cells["B" + (i + 8)].Value = item.Name;
                worksheet.Cells["C" + (i + 8)].Value = item.Price;
                using (var range = worksheet.Cells["A" + (i + 8) + ":C" + i + 8])
                {
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
                worksheet.Row(i + 8).Height = 30;
            }

            using (var range = worksheet.Cells["A" + (model.ListServiceDental.Count + 1) + ":C" + model.ListServiceDental.Count + 1])
            {
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Row(model.ListServiceDental.Count + 1).Height = 30;
            }
            for (int i = 1; i <= worksheet.Dimension.End.Column; i++) { worksheet.Column(i).AutoFit(); }
            worksheet.Cells[1, 1, model.ListServiceDental.Count + 10, 9].AutoFitColumns(20);
            worksheet.Cells["A8:A" + model.ListServiceDental.Count + 8].AutoFitColumns(10);
            worksheet.Cells["D8:D" + model.ListServiceDental.Count + 8].AutoFitColumns(40);
            worksheet.Cells["D7:D7"].AutoFilter = true;

            int currentRoomLength = model.ListServiceDental.Count;


        }

        private Stream CreateExcelFileOrder(ExportUserAppointment model, Stream stream = null)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                excelPackage.Workbook.Properties.Author = "NhaKhoaPhuongThao";
                excelPackage.Workbook.Properties.Title = "Create Excel File";
                excelPackage.Workbook.Properties.Comments = "Lịch hẹn mã " + model.UserAppointmentInfo.AppointmentCode;
                excelPackage.Workbook.Worksheets.Add("Trang 1");
                var workSheet = excelPackage.Workbook.Worksheets[0];
                BindingFormatForExcelOrder(workSheet, model);

                excelPackage.Save();
                return excelPackage.Stream;
            }
        }
        [HttpPost]
        public HttpResponseMessage ExportFileExcelOrder(ExportUserAppointment model)
        {
            using (var streams = CreateExcelFileOrder(model, null) as MemoryStream)
            {
                using (var memoryStream = new MemoryStream())
                {
                    var nameFile = "";
                    streams.Position = 0;
                    streams.CopyTo(memoryStream);
                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(memoryStream.ToArray())
                    };
                    nameFile = "Lichhenma" + model.UserAppointmentInfo.AppointmentCode;
                    result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = nameFile + ".xlsx"
                    };
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    return result;
                }
            }
        }

        [HttpPost]
        public HttpResponseMessage ExportPdf(ExportUserAppointment model)
        {
            BaseFont font = BaseFont.CreateFont(@"C:\Windows\Fonts\Arial.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            iTextSharp.text.Font vietnameseFont = new iTextSharp.text.Font(font, 12, iTextSharp.text.Font.NORMAL);


            BaseFont fontContent = BaseFont.CreateFont(@"C:\Windows\Fonts\Arial.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            iTextSharp.text.Font vietnameseFontContent = new iTextSharp.text.Font(font, 16, iTextSharp.text.Font.NORMAL);

            // Tạo một đối tượng Document mới
            iTextSharp.text.Document document = new iTextSharp.text.Document();
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);

            // Mở tài liệu
            document.Open();

            // Create a PdfContentByte object
            PdfContentByte cb = writer.DirectContent;

            // Add content to the document
            PdfPCell cell = new PdfPCell(new Paragraph("HÓA ĐƠN ĐẶT HẸN", vietnameseFontContent));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderWidth = 0;
            PdfPTable table = new PdfPTable(1);
            table.AddCell(cell);
            document.Add(table);


            // Add order info
            PdfPTable tableOrderInfo = new PdfPTable(2);
            PdfPCell cellOrderInfo = new PdfPCell();
            cellOrderInfo = new PdfPCell(new Paragraph("Họ và tên khách hàng: " + model.UserAppointmentInfo.Name, vietnameseFont));
            cellOrderInfo.BorderWidth = 0;
            cellOrderInfo.PaddingTop = 10f;
            cellOrderInfo.PaddingBottom = 10f;
            tableOrderInfo.AddCell(cellOrderInfo);

            cellOrderInfo = new PdfPCell(new Paragraph("Mã lịch hẹn: " + model.UserAppointmentInfo.AppointmentCode, vietnameseFont));
            cellOrderInfo.BorderWidth = 0;
            cellOrderInfo.PaddingTop = 10f;
            cellOrderInfo.PaddingBottom = 10f;
            tableOrderInfo.AddCell(cellOrderInfo);

            cellOrderInfo = new PdfPCell(new Paragraph("Số điện thoại: " + model.UserAppointmentInfo.Phone, vietnameseFont));
            cellOrderInfo.BorderWidth = 0;
            cellOrderInfo.PaddingTop = 10f;
            cellOrderInfo.PaddingBottom = 10f;
            tableOrderInfo.AddCell(cellOrderInfo);

            //cellOrderInfo = new PdfPCell(new Paragraph("Ngày đến: " + model.UserAppointmentInfo.Hour, vietnameseFont));
            //cellOrderInfo.BorderWidth = 0;
            //cellOrderInfo.PaddingTop = 10f;
            //cellOrderInfo.PaddingBottom = 10f;
            //tableOrderInfo.AddCell(cellOrderInfo);

            cellOrderInfo = new PdfPCell(new Paragraph("Email: " + model.UserAppointmentInfo.Email, vietnameseFont));
            cellOrderInfo.BorderWidth = 0;
            cellOrderInfo.PaddingTop = 10f;
            cellOrderInfo.PaddingBottom = 10f;
            tableOrderInfo.AddCell(cellOrderInfo);

            //cellOrderInfo = new PdfPCell(new Paragraph("Ngày đi: " + model.UserAppointmentInfo.Day, vietnameseFont));
            //cellOrderInfo.BorderWidth = 0;
            //cellOrderInfo.PaddingTop = 10f;
            //cellOrderInfo.PaddingBottom = 10f;
            //tableOrderInfo.AddCell(cellOrderInfo);

            tableOrderInfo.SpacingBefore = 20f;
            tableOrderInfo.SpacingAfter = 10f;
            tableOrderInfo.WidthPercentage = 100f;
            tableOrderInfo.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            document.Add(tableOrderInfo);


            // Table Room Category
            PdfPTable tableRoom = new PdfPTable(3);
            float[] widths = new float[] { 1f, 2f, 1f };
            tableRoom.SetWidths(widths);
            PdfPCell cellRoom = new PdfPCell();
            cellRoom = new PdfPCell(new Paragraph("STT", vietnameseFont));
            cellRoom.HorizontalAlignment = Element.ALIGN_CENTER;
            cellRoom.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellRoom.PaddingTop = 10f;
            cellRoom.PaddingBottom = 10f;
            tableRoom.AddCell(cellRoom);

            cellRoom = new PdfPCell(new Paragraph("Dịch vụ", vietnameseFont));
            cellRoom.HorizontalAlignment = Element.ALIGN_CENTER;
            cellRoom.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellRoom.PaddingTop = 10f;
            cellRoom.PaddingBottom = 10f;
            tableRoom.AddCell(cellRoom);


            cellRoom = new PdfPCell(new Paragraph("Giá", vietnameseFont));
            cellRoom.HorizontalAlignment = Element.ALIGN_CENTER;
            cellRoom.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellRoom.PaddingTop = 10f;
            cellRoom.PaddingBottom = 10f;
            tableRoom.AddCell(cellRoom);

            for (int i = 0; i < model.ListServiceDental.Count; i++)
            {
                var item = model.ListServiceDental[i];
                cellRoom = new PdfPCell(new Paragraph("" + (i + 1), vietnameseFont));
                 cellRoom.HorizontalAlignment = Element.ALIGN_CENTER;
                cellRoom.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellRoom.PaddingTop = 10f;
                cellRoom.PaddingBottom = 10f;
                tableRoom.AddCell(cellRoom);

                cellRoom = new PdfPCell(new Paragraph("" + item.Name, vietnameseFont));
                cellRoom.HorizontalAlignment = Element.ALIGN_CENTER;
                 cellRoom.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellRoom.PaddingTop = 10f;
                cellRoom.PaddingBottom = 10f;
                tableRoom.AddCell(cellRoom);

                cellRoom = new PdfPCell(new Paragraph("" + item.Price, vietnameseFont));
                cellRoom.HorizontalAlignment = Element.ALIGN_CENTER;
                cellRoom.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellRoom.PaddingTop = 10f;
                cellRoom.PaddingBottom = 10f;
                tableRoom.AddCell(cellRoom);
            }


            tableRoom.SpacingBefore = 20f;
            tableRoom.SpacingAfter = 10f;
            tableRoom.WidthPercentage = 100f;
            tableRoom.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            document.Add(tableRoom);



            // Người lập hóa đơn
            PdfPTable table1 = new PdfPTable(1);
            table1.SpacingBefore = 10f;
            table1.SpacingAfter = 10f;

            PdfPCell cell1 = new PdfPCell(new Paragraph("Tổng tiền: " + model.UserAppointmentInfo.TotalAmount, vietnameseFont));
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.BorderWidth = 0;
            table1.AddCell(cell1);

            cell1 = new PdfPCell(new Paragraph("Người lập hóa đơn", vietnameseFont));
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.BorderWidth = 0;
            cell1.PaddingTop = 30f;
            table1.AddCell(cell1);

            cell1 = new PdfPCell(new Paragraph("Nguyễn Hữu Tuấn", vietnameseFont));
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.BorderWidth = 0;
            cell1.PaddingTop = 30f;
            table1.AddCell(cell1);

            document.Add(table1);

            // Đóng tài liệu
            document.Close();

            // Trả về tệp PDF dưới dạng phản hồi HTTP
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(stream.ToArray());
            response.Content.Headers.Add("Content-Type", "application/pdf");
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = "invoice.pdf";
            return response;
        }
    }
}
