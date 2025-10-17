using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using DocuSign.eSign.Model;
using Microsoft.IdentityModel.Tokens;
using OnlineFruit_Business.Option;
using MimeKit;
using MimeKit.Text;
using MailKit.Security;
using Microsoft.Extensions.Hosting;

namespace OnlineFruit_Business.Email
{
    public interface IEmailService
    {
        Task SendEmail(string reseptor, string body,string subject);
    }
    public class EmailSender : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            
            _configuration = configuration;
        }

        public async Task SendEmail(string subject, string body , string reseptor)
        {
            //var receiverEmail = "tayeb.mora69@gmail.com";
            ////// ایجاد لینک تغییر رمز عبور
            ////string resetLink = $"https://yourwebsite.com/ResetPassword?email={receiverEmail}&token={reseptor}";

            ////// قالب HTML ایمیل
            ////string emailBody = $@"
            ////<html>
            ////<body>
            ////    <h2>درخواست تغییر رمز عبور</h2>
            ////    <p>برای تغییر رمز عبور روی لینک زیر کلیک کنید:</p>
            ////    <a href='{resetLink}'>تغییر رمز عبور</a>
            ////    <p>اگر این درخواست از طرف شما نبوده، این ایمیل را نادیده بگیرید.</p>
            ////</body>
            ////</html>";

            var email = _configuration.GetValue<string>("EMAIL_CONFIGORATION:EMAIL");
            var password = _configuration.GetValue<string>("EMAIL_CONFIGORATION:PASSWORD");
            var host = _configuration.GetValue<string>("EMAIL_CONFIGORATION:HOST");
            var port = _configuration.GetValue<int>("EMAIL_CONFIGORATION:PORT");

            //var smtpClient = new SmtpClient(host,port);
            //smtpClient.EnableSsl = true;
            //smtpClient.UseDefaultCredentials = false;

            //smtpClient.Credentials = new NetworkCredential(email, password);
            //var mailMessage = new MailMessage();
            //mailMessage.From = new MailAddress(email); // ایمیل معتبر
            //mailMessage.To.Add(new MailAddress(receiverEmail)); // گیرنده معتبر

            //mailMessage.IsBodyHtml = true;
            ////var message = new MailMessage(email,reseptor,subject,body);
            //mailMessage.From = new MailAddress("tayeb.mora69@gmail.com"); // ایمیل معتبر
            //await smtpClient.SendMailAsync(mailMessage);

            using var smtpClient = new SmtpClient(host, port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(email, password)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(email), // ایمیل فرستنده
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(new MailAddress(reseptor)); // اضافه کردن گیرنده

            await smtpClient.SendMailAsync(mailMessage); // ارسال ایمیل
        }
    }

}
