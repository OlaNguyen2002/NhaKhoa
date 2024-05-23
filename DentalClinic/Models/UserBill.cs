using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DentalClinic.Models
{
    public class UserBill
    {
        public string UserBillId { get; set; }
        public string UserId { get; set; }
        public string UserAppointmentId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public decimal TotalAmount { get; set; }
        public long CreateTime { get; set; }

    }
}