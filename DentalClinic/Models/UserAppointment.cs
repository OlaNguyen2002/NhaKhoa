using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DentalClinic.Models
{
    public class UserAppointment
    {
        public string UserAppointmentId { get; set; }
        public string UserId { get; set; }
        public string AppointmentCode { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string DoctorId { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string Status { get; set; }
        public long CreateTime { get; set; }
        public int TotalExpectTime { get; set; }
        public decimal TotalAmount { get; set; }
        public class EnumStatus
        {
            public const string PENDING = "PENDING";
            public const string RECEIVE = "RECEIVE";
            public const string DONE = "DONE";
            public const string CONFIRM_ARRIVE = "CONFIRM_ARRIVE";
            public const string USER_CANCEL = "USER_CANCEL";
            public const string SYSTEM_CANCEL = "SYSTEM_CANCEL";
        }
    }
    public class UserAppointmentInfor
    {
        public string UserAppointmentId { get; set; }
        public string UserId { get; set; }
        public string AppointmentCode { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string DoctorId { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string Status { get; set; }
        public long CreateTime { get; set; }
        public int TotalExpectTime { get; set; }
        public List<ServiceDental> lsServiceDental { get; set; }

        public class EnumStatus
        {
            public const string PENDING = "PENDING";
            public const string RECEIVE = "RECEIVE";
            public const string DONE = "DONE";
            public const string CONFIRM_PAID = "CONFIRM_PAID";
            public const string USER_CANCEL = "USER_CANCEL";
            public const string SYSTEM_CANCEL = "SYSTEM_CANCEL";
        }
        public string NameDoctor { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class ListUserAppointmentView
    {
        public List<UserAppointmentInfor> ListUserAppointmentInfor { get; set; }
        public int TotalPage { get; set; }

    }
    public class UserAppointmentRequest
    {
        public string UserId { get; set; }
        public string DoctorId { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public List<ServiceDental> ListServiceId { get; set; }
    }
    public class ExportUserAppointment
    {
        public UserAppointmentInfor UserAppointmentInfo { get; set; }
        public List<ServiceDental> ListServiceDental { get; set; }
    }
}