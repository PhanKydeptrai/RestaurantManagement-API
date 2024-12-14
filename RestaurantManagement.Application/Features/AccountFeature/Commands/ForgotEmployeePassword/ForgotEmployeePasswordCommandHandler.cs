using System.Net;
using System.Net.Mail;
using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ForgotEmployeePassword;

public class ForgotEmployeePasswordCommandHandler(
    IEmployeeRepository employeeRepository,
    IUnitOfWork unitOfWork,
    IFluentEmail fluentEmail,
    IEmailVerify emailVerify,
    IApplicationDbContext context) : ICommandHandler<ForgotEmployeePasswordCommand>
{
    public async Task<Result> Handle(ForgotEmployeePasswordCommand request, CancellationToken cancellationToken)
    {
        //Validate request
        var validator = new ForgotEmployeePasswordCommandValidator(employeeRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        //Get user by email
        Ulid userId = await context.Employees
            .Where(a => a.User.Email == request.email)
            .Select(a => a.UserId)
            .FirstOrDefaultAsync();

        //Create email verification token
        var emailVerificationToken = new EmailVerificationToken
        {
            EmailVerificationTokenId = Ulid.NewUlid(),
            ExpiredDate = DateTime.UtcNow.AddMinutes(5),
            UserId = userId,
            CreatedDate = DateTime.UtcNow
        };

        //FIX: Xử lý hard code
        //gửi mail xác thực
        var verificationLink = emailVerify.CreateLinkForResetPass(emailVerificationToken);

        bool emailSent = false;
        int retryCount = 0;
        int maxRetries = 5;

        do
        {
            try
            {
                #region Send Email using Gmail SMTP
                // Thông tin đăng nhập và cài đặt máy chủ SMTP
                string fromEmail = "nhumnhumrestaurant@gmail.com"; // Địa chỉ Gmail của bạn
                string toEmail = request.email;  // Địa chỉ người nhận
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
                    Subject = "Nhà hàng Nhum nhum - Nhận mật khẩu mới",
                    Body = $"Vui lòng nhấn vào link sau để nhận mật khẩu mới: <a href='{verificationLink}'>Click me</a> <br>Link chỉ có hiệu lực trong 5 phút",
                    IsBodyHtml = true // Nếu muốn gửi email ở định dạng HTML
                };

                mailMessage.To.Add(toEmail);

                // Gửi email
                smtpClient.Send(mailMessage);
                #endregion
                emailSent = true;
            }
            catch
            {
                retryCount++;
                if (retryCount >= maxRetries)
                {
                    return Result.Failure(new[] { new Error("Email", "Failed to send email") });
                }
            }
        }
        while (!emailSent && retryCount < maxRetries);


        await context.EmailVerificationTokens.AddAsync(emailVerificationToken);

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
