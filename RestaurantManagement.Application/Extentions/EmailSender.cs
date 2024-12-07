using System.Net;
using System.Net.Mail;
using FluentEmail.Core;
using Microsoft.Extensions.Configuration;

namespace RestaurantManagement.Application.Extentions;

public static class EmailSender
{
    private static IConfiguration _configuration;

    public static async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        try
        {
            // Cấu hình SMTP client để gửi email qua Gmail

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = int.Parse(_configuration["Port"]!), // Cổng SMTP cho Gmail với TLS
                Credentials = new NetworkCredential(_configuration["RestaurantEmail"], _configuration["AppPasswords"]), // Mật khẩu ứng dụng ở đây
                EnableSsl = true // Sử dụng SSL
            };

            // Tạo đối tượng email
            MailMessage mail = new MailMessage
            {
                From = new MailAddress(_configuration["RestaurantEmail"]!), // Mail người gửi
                Subject = "Test Email từ .NET",
                Body = "Đây là email thử nghiệm gửi từ ứng dụng .NET.",
                IsBodyHtml = true
            };
            // Thêm người nhận email
            mail.To.Add(email); // Thay thế với địa chỉ email người nhận

            // Gửi email
            smtpClient.Send(mail);

        }
        catch (Exception ex)
        {

        }
    }
}
