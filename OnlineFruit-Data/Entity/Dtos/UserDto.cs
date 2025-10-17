using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineFruit_Data.Entity.Dtos
{
    public class UserDto 
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmailOrPhone { get; set; } // برای لاگین با ایمیل یا شماره موبایل
        public string? Role { get; set; }
        public List<string>? Roles { get; set; }
        public bool IsCustomer { get; set; } // آیا این کاربر مشتری است؟

    }
}
