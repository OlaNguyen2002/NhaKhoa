using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DentalClinic.Models
{
    public class CustomerFeedback
    {
        public int CustomerFeedbackId { get; set; }
        public string CustomerName { get; set; }
        public string Avatar { get; set; }
        public string Contents { get; set; }
    }
}