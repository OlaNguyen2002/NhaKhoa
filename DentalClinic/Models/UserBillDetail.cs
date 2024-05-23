using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DentalClinic.Models
{
    public class UserBillDetail
    {
        public string UserBillDetailId { get; set; }
        public string UserBillId { get; set; }
        public string ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal Price { get; set; }
    }
}