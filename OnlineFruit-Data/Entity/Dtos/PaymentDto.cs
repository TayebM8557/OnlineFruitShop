using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruit_Data.Entity.Dtos
{
    public class PaymentDto 
    {
        public int? Id { get; set; }
        public DateTime? PaymentDate { get; set; } // تاریخ پرداخت
        public decimal? Amount { get; set; } // مبلغ پرداختی
        public string? PaymentMethod { get; set; } // روش پرداخت
     
        public string? TransactionId { get; set; } // شماره تراکنش

        public int? OrderId { get; set; }
        public Order? Order { get; set; }
    }
    
}
