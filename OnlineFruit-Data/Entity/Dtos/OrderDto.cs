using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruit_Data.Entity.Dtos
{
    public class OrderDto
    {
        public int? Id { get; set; }
        public DateTime? OrderDate { get; set; }
        
        public decimal? TotalAmount { get; set; } // مبلغ کل
        public Payment? Payment { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public string? TrackingCode { get; set; } // کد رهگیری
        public string? Notes { get; set; } // توضیحات سفارش
    }
   
}
