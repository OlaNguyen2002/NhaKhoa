using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DentalClinic.Models
{
    public class News
    {
        public int NewsId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string SearchTitle { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public string NewsContent { get; set; }
        public long CreateTime { get; set; }
        public bool Enable { get; set; }
    }
    public class NewsListViewModel
    {
        public List<News> ListNews { get; set; }
        public int TotalPage { get; set; }

    }

}