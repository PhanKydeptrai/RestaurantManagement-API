using System.Net;
using System.Net.Mail;
using FluentEmail.Core;
using Microsoft.Extensions.Configuration;
using NETCore.Encrypt;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ResetPasswordVerify;

public class ResetPasswordVerifyCommandHandler(
    IEmailVerificationTokenRepository emailVerificationTokenRepository,
    IEmailVerify emailVerify,
    IApplicationDbContext applicationDbContext,
    IUnitOfWork unitOfWork,
    IApplicationDbContext context,
    IFluentEmail fluentEmail,
    IUserRepository userRepository,
    IConfiguration configuration) : ICommandHandler<ResetPasswordVerifyCommand>
{
    public async Task<Result> Handle(ResetPasswordVerifyCommand request, CancellationToken cancellationToken)
    {
        EmailVerificationToken token = await emailVerificationTokenRepository.GetVerificationTokenById(request.tokenId);
        if (token is null) //kiểm tra token có tồn tại không
        {
            Error[] errors = new[]
            {
                new Error("EmailVerificationToken", "Link is invalid")
            };
            return Result.Failure(errors);
        }

        if (token.ExpiredDate < DateTime.UtcNow) //kiểm tra thời gian hết hạn của token
        {
            Error[] errors = new[]
            {
                new Error("EmailVerificationToken", "Link is invalid")
            };
            return Result.Failure(errors);
        }
        //Tạo một mật khẩu mới
        string randomPass = RandomStringGenerator.GenerateRandomString();
        var user = await userRepository.GetUserById(token.UserId);
        user.Password = EncryptProvider.Sha256(randomPass);

        // Gửi email thông báo
        bool emailSent = false;
        int retryCount = 0;
        int maxRetries = 5;

        do
        {
            if (configuration["Environment"] == "Development")
            {
                try
                {

                    await fluentEmail.To(token.User.Email).Subject("Nhà hàng Nhum nhum - Thông báo mật khẩu mới")
                        .Body($"Mật khẩu mới của bạn là: {randomPass}", isHtml: true)
                        .SendAsync();

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
            else
            {
                try
                {
                    #region Send Email using Gmail SMTP
                    // Thông tin đăng nhập và cài đặt máy chủ SMTP
                    string fromEmail = "nhumnhumrestaurant@gmail.com"; // Địa chỉ Gmail của bạn
                    string toEmail = user.Email;  // Địa chỉ người nhận
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
                        Subject = "Nhà hàng Nhum nhum - Xác nhận thay đổi mật khẩu",
                        Body = $"Mật khẩu mới của bạn là: {randomPass}",
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
        }
        while (!emailSent && retryCount < maxRetries);

        //Xóa token
        emailVerificationTokenRepository.RemoveVerificationToken(token);
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}

