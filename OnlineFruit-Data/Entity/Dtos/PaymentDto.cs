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
        public int Id { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } // روش پرداخت (مثلاً کارت بانکی، کیف پول)

        public int? OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
