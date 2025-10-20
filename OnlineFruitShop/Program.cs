using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using OnlineFruit_Business;
using OnlineFruit_Business.Email;
using OnlineFruit_Business.Option;
using OnlineFruit_Business.Repository.IReposi;
using OnlineFruit_Business.Repository.Reposi;
using OnlineFruit_Business.Utilities;
using OnlineFruit_Data.Context;
using System.Reflection;
using static OnlineFruit_Data.Entity.APP;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings

    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.SlidingExpiration = true;
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/AccessDenied";

});
// تنظیمات SMTP
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("SmtpOption"));
builder.Services.AddTransient<IEmailService, EmailSender>();

// تنظیمات درگاه پرداخت
builder.Services.Configure<OnlineFruit_Business.Payment.PaymentGatewaySettings>(
    builder.Configuration.GetSection("PaymentGateway"));
builder.Services.AddHttpClient<OnlineFruit_Business.Payment.ZarinpalPaymentService>();
builder.Services.AddScoped<OnlineFruit_Business.Payment.IPaymentService, OnlineFruit_Business.Payment.ZarinpalPaymentService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderRepository ,OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository,ProductRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(AutoMapping)));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "userinfo";
});

//builder.Services.Configure<SmtpOption63>(builder.Configuration.GetSection("SmtpOption"));

//builder.Services.Configure<IdentityOptions>(options =>
//{
//    options.SignIn.RequireConfirmedEmail = true; // تأیید ایمیل را اجباری کنید
//});


builder.Services.AddIdentity<OnlineFruit_Data.Entity.APP.User, IdentityRole<int>>(
        options =>
        {
            options.SignIn.RequireConfirmedEmail = false; // غیرفعال کردن تایید ایمیل
            options.SignIn.RequireConfirmedPhoneNumber = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 4;
            options.Password.RequireNonAlphanumeric = false;

        }
    )
    .AddEntityFrameworkStores<DatabaseContext>()
    .AddErrorDescriber<PersianIdentityErrorDescriber>().AddDefaultTokenProviders();
builder.Services.AddSession();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin",
        policy => policy.RequireRole("Admin"));
    options.AddPolicy("Customer",
       policy => policy.RequireRole("Customer"));

});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();



app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
