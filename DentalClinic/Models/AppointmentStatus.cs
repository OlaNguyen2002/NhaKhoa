using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DentalClinic.Models
{
    public class AppointmentStatus
    {
        public string AppointmentStatusId { get; set; }
        public string UserAppointmentId { get; set; }
        public string Status { get; set; }
        public long CreateTime { get; set; }
    }
}