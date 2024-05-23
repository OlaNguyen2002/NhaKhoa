using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DentalClinic.Models
{
    public class UserFeedback
    {
        public int UserFeedbackId { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }

        public string Contents { get; set; }
        public int RateStar { get; set; }
    }
}