using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DentalClinic.Models
{
    public class Slide
    {
        public string SlideId { get; set; }
        public string Image { get; set; }
        public string LinkWeb { get; set; }
        public string Area { get; set; }
        public class EnumArea
        {
            public const string HOME_SLIDE = "home-slide";
            public const string BANNER_AREA_1 = "banner-1";
        }

    }
    public class SlideListViewModel
    {
        public List<Slide> ListSlide { get; set; }
        public int TotalPage { get; set; }
    }
}