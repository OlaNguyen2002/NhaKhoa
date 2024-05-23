using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DentalClinic.Models
{
    public class GoodsReceipt
    {
        public string GoodsReceiptId { get; set; }
        public string Supplier { get; set; }
        public string Status { get; set; }
        public decimal TotalAount { get; set; }
        public long CreateTime { get; set; }
    }
}