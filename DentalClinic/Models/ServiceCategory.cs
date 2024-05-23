using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DentalClinic.Models
{
    public class ServiceCategory
    {
        public string ServiceCategoryId { get; set; }
        public string Name { get; set; }
        public bool Enable { get; set; }
        public bool ShowInHomePage { get; set; }
    }
    public class ServiceCategoryUpdate
    {
        public string ServiceCategoryId { get; set; }
        public string Name { get; set; }
    }
}