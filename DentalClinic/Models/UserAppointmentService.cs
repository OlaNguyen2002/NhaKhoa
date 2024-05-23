using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DentalClinic.Models
{
    public class UserAppointmentService
    {
        public string UserAppointmentServiceId { get; set; }
        public string UserAppointmentId { get; set; }
        public string ServiceId { get; set; }
        public int ExpectTime { get; set; }

    }
    public class UserAppointmentServiceUpdate : UserAppointmentService
    {
        public string NameService{ get; set; }
        public decimal PriceService { get; set; }
    }
}