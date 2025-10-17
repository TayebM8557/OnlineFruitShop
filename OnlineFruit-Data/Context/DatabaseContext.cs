using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OnlineFruit_Data.Entity.APP;
using Microsoft.EntityFrameworkCore;

namespace OnlineFruit_Data.Context
{
    public class DatabaseContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        } 
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Payment> Payments { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasOne(u => u.Address)
                .WithMany(a => a.Users)
                .HasForeignKey(u => u.AddressId);
            base.OnModelCreating(builder);

            // Seed Categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "میوه و سبزیجات" },
                new Category { Id = 2, Name = "نوشیدنی" },
                new Category { Id = 3, Name = "لبنیات" },
                new Category { Id = 4, Name = "نان و شیرینی" }
            );

            // Seed Roles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2", Name = "Customer", NormalizedName = "Customer" }
            );

            // Seed Example Products
            builder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "سیب",
                    Description = "سیب تازه و خوشمزه",
                    Price = 12000,
                    Stock = 50,
                    ImageUrl = "apple.jpg",
                    CategoryId = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Discount = 0
                },
                new Product
                {
                    Id = 2,
                    Name = "شیر",
                    Description = "شیر تازه دامداری",
                    Price = 15000,
                    Stock = 30,
                    ImageUrl = "milk.jpg",
                    CategoryId = 3,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Discount = 0
                }
            );
        }
    }

    
}
 