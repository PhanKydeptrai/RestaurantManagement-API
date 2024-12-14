using System.Net;
using System.Net.Mail;

namespace RestaurantManagement.Application.Extentions;

public static class EmailSender
{
    //FIX hardcode
    public static void SendEmailAsync(string email, string subject, string body)
    {
        #region Send Email using Gmail SMTP
        // Thông tin đăng nhập và cài đặt máy chủ SMTP
        string fromEmail = "nhumnhumrestaurant@gmail.com"; // Địa chỉ Gmail của bạn
        string toEmail = email;  // Địa chỉ người nhận
        string password = "ekgh lntd brrv bdyj";   // Mật khẩu ứng dụng (nếu bật 2FA) hoặc mật khẩu của tài khoản Gmail

        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587, // Cổng sử dụng cho TLS
            Credentials = new NetworkCredential(fromEmail, password), // Đăng nhập vào Gmail
            EnableSsl = true // Kích hoạt SSL/TLS
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(fromEmail),
            Subject = subject,
            Body = body,
            IsBodyHtml = true // Nếu muốn gửi email ở định dạng HTML
        };

        mailMessage.To.Add(toEmail);

        // Gửi email
        smtpClient.Send(mailMessage);
        #endregion
    }
}
