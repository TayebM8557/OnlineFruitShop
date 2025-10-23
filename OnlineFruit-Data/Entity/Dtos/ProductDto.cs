using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruit_Data.Entity.Dtos
{
    public class ProductDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; } // قیمت محصول
        public int? Stock { get; set; } // موجودی انبار
        public string? ImageUrl { get; set; } // آدرس تصویر محصول
       

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; } // برای قفل خوش‌بینانه

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public decimal? Discount { get; set; } // اضافه کردن فیلد تخفیف برای هر محصول

    }

   
}
