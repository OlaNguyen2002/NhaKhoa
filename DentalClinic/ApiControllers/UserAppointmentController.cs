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

namespace DentalClinic.ApiControllers
{
    public class UserAppointmentController : ApiBaseController
    {
        [HttpPost]
        [ApiTokenRequire]
        public JsonResult CreateUserAppointment(UserAppointmentRequest model)
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
                        UserMakeAppointmentService userMakeAppointmentService = new UserMakeAppointmentService(connect);
                        ServiceService serviceService = new ServiceService(connect);

                        
                        string token = Request.Headers.Authorization.ToString();
                        User user = userService.GetUserByToken(token, transaction);
                        if (user == null) return Unauthorized();
                        if (user.Name == null || user.Phone == null || user.Email == null || user.Address == null) throw new Exception("Bạn vui lòng cập nhật đầy đủ thông tin tài khoản");

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
                       for (int i = 0; i<model.ListServiceId.Count; i++)
                        {
                            string ServiceId = model.ListServiceId[i].ServiceId;
                            ServiceDental serviceDental = serviceService.GetServiceById(ServiceId, transaction);
                            if (serviceDental == null) return Error("Dịch vụ này không tồn tại");
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
                        return Success(new { AppointmentCode = userAppointment.AppointmentCode});
                    }
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        [ApiTokenRequire]
        public JsonResult GetListAppointmentOfUser(int page, string keyword)
        {
            try
            {
                User user = SecurityProvider.GetUserByToken(Request);
                if (user == null) return Unauthorized();
                UserMakeAppointmentService userMakeAppointmentService = new UserMakeAppointmentService();
                ServiceService serviceService = new ServiceService();

                ListUserAppointmentView listUserAppointmentView = userMakeAppointmentService.GetListAppointmentOfUser(page, keyword, user.UserId);
                for (int i=0; i < listUserAppointmentView.ListUserAppointmentInfor.Count; i++) {
                    var item = listUserAppointmentView.ListUserAppointmentInfor[i];
                    List<ServiceDental> lsServiceDental = serviceService.GetServiceByUserAppointmentId(item.UserAppointmentId);
                    decimal totalPriceService = 0;
                    foreach (var _item in lsServiceDental) {
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
        [ApiTokenRequire]
        public JsonResult GetUserById(string userId)
        {
            try
            {
                User user = SecurityProvider.GetUserByToken(Request);
                if (user == null) return Unauthorized();
                UserService userService = new UserService();
                return Success(userService.GetUserById(userId));

            }
            catch (Exception ex)
            {
                return Error(ex.Message);

            }
        }

        [HttpGet]
        [ApiTokenRequire]
        public JsonResult GetAppointmentDetailOfUSer(string appointmentCode)
        {
            try
            {
                User user = SecurityProvider.GetUserByToken(Request);
                if (user == null) return Unauthorized();

                UserMakeAppointmentService userMakeAppointmentService = new UserMakeAppointmentService();
                UserAppointment userAppointment = userMakeAppointmentService.GetAppointmentByAppointmentCode(appointmentCode);
                if (userAppointment == null) return Error();

                List<UserAppointmentServiceUpdate> listUserAppointmentService = userMakeAppointmentService.GetListUserAppointmentServiceUpdateByUserAppointmentId(userAppointment.UserAppointmentId);
                return Success(new { userAppointment, listUserAppointmentService });
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        [ApiTokenRequire]
        public JsonResult UserCancelAppointment(string userAppointmentId)
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
                        string token = Request.Headers.Authorization.ToString();
                        User user = userService.GetUserByToken(token, transaction);
                        if (user == null) return Unauthorized();

                        // Lấy ra 
                        UserAppointment userAppointment = userMakeAppointmentService.GetUserAppointmentById(userAppointmentId, transaction);
                        if (userAppointment == null) throw new Exception("Không tìm thấy lịch hẹn !");
                        if (userAppointment.UserId != user.UserId) throw new Exception("Lịch hẹn này không phải của bạn !");
                        if (userAppointment.Status != UserAppointment.EnumStatus.PENDING) throw new Exception("Trạng thái lịch hẹn không hợp lệ !");

                        // Set lại trạng thái 
                        if (!userMakeAppointmentService.UpdateUserAppointmentStatus(userAppointmentId, UserAppointment.EnumStatus.USER_CANCEL, transaction)) throw new Exception();

                        // Lưu lịch sử status
                        AppointmentStatus appointmentStatus = new AppointmentStatus();
                        appointmentStatus.AppointmentStatusId = Guid.NewGuid().ToString();
                        appointmentStatus.UserAppointmentId = userAppointmentId;
                        appointmentStatus.Status = UserAppointment.EnumStatus.USER_CANCEL;
                        appointmentStatus.CreateTime = HelperProvider.GetSeconds();
                        if (!appointmentStatusService.CreateAppointmentStatus(appointmentStatus, transaction)) throw new Exception();

                        // Thông báo cho người dùng
                        Notification notification = new Notification();
                        notification.UserId = user.UserId;
                        notification.Title = "Thông báo";
                        notification.Message = "Bạn đã hủy lịch hẹn: " + userAppointment.AppointmentCode;
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

    }
}
