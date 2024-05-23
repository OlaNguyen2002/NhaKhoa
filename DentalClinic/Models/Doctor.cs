using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DentalClinic.Models
{
    public class Doctor
    {
        public string DoctorId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Avatar { get; set; }
        public string Qualification { get; set; }
        public bool Enable { get; set; }
        public long CreateTime { get; set; }
        public string SearchName { get; set; }
    }
    public class DoctorListViewModel
    {
        public List<Doctor> ListDoctor { get; set; }
        public int TotalPage { get; set; }

    }
    public class TimeDoctor {
        public int MinTime { get; set; }
        public int MaxTime { get; set; }

    }
}