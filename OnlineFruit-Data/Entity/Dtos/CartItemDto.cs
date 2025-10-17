using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruit_Data.Entity.Dtos
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }

        public int? ProductId { get; set; }
        public Product? Product { get; set; }
     
        public int Quantity { get; set; } // تعداد محصول
    }
}
