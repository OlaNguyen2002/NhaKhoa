using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DentalClinic.Models
{
    public class SystemTransaction
    {
        public string SystemTransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Message { get; set; }
        public long CreateTime { get; set; }
    }
}