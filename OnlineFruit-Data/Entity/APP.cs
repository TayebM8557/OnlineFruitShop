using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OnlineFruit_Data.Entity
{
    public class APP
    {
        public class User : IdentityUser<int>
        {
            public bool IsCustomer { get; set; } // آیا این کاربر مشتری است؟
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public ICollection<Order> Orders { get; set; }     // لیست سفارش‌ها
            public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

            public int? AddressId { get; set; }
            public Address? Address { get; set; }
        }
       
        [Owned]
        public class Address
        {
            public int Id { get; set; }
            public string Street { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string PostalCode { get; set; }
            public string Country { get; set; }
            public int plaque { get; set; }
            public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

            public ICollection<User> Users { get; set; } 
            
        }
        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; } // قیمت محصول
            public int Stock { get; set; } // موجودی انبار
            public string ImageUrl { get; set; } // آدرس تصویر محصول

            public int CategoryId { get; set; }
            public Category Category { get; set; }

            [Timestamp]
            public byte[] RowVersion { get; set; } // برای قفل خوش‌بینانه

            public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
            public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
            public decimal Discount { get; set; } // اضافه کردن فیلد تخفیف برای هر محصول
        }
       
        public class CartItem
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public User User { get; set; }

            public int ProductId { get; set; }
            public Product Product { get; set; }

            public int Quantity { get; set; } // تعداد محصول
            public DateTime AddedAt { get; set; } // تاریخ اضافه شدن
            public DateTime? UpdatedAt { get; set; } // تاریخ بروزرسانی
        }

        public class Order
        {
            public int Id { get; set; }
            public DateTime OrderDate { get; set; }
            public OrderStatus Status { get; set; } = OrderStatus.Pending;
            public decimal TotalAmount { get; set; } // مبلغ کل

            public int UserId { get; set; }
            public User User { get; set; }
            public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
            public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        }

        public enum OrderStatus
        {
            Pending,      // در انتظار
            Processing,   // در حال پردازش
            Shipped,      // ارسال شده
            Delivered,    // تحویل داده شده
            Cancelled     // لغو شده
        }
        public class OrderItem
        {
            public int Id { get; set; }

            public int OrderId { get; set; }
            public Order Order { get; set; }

            public int ProductId { get; set; }
            public Product Product { get; set; }
            public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
            public int Quantity { get; set; } // تعداد محصول
            public decimal UnitPrice { get; set; } // قیمت هر واحد
            public decimal Discount { get; set; } // تخفیف برای این آیتم
            public decimal TotalPrice => (UnitPrice - Discount) * Quantity; // قیمت کل
        }
        public class Category
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public ICollection<Product> Products { get; set; }
        }
        public class Payment
        {
            public int Id { get; set; }
            public DateTime PaymentDate { get; set; } // تاریخ پرداخت
            public decimal Amount { get; set; } // مبلغ پرداختی
            public string PaymentMethod { get; set; } // روش پرداخت
            public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
            public string TransactionId { get; set; } // شماره تراکنش

            public int OrderId { get; set; }
            public Order Order { get; set; }
        }

        public enum PaymentStatus
        {
            Pending,    // در انتظار
            Completed,  // تکمیل شده
            Failed,     // ناموفق
            Refunded    // بازگشت داده شده
        }
    }
}
