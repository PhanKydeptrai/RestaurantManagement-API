using System.Net;
using System.Net.Mail;
using FluentEmail.Core;
using Microsoft.Extensions.Configuration;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.DeleteCustomerAccount;

public class DeleteCustomerAccountCommandHandler(
    ICustomerRepository customerAccountRepository,
    IUnitOfWork unitOfWork,
    IEmailVerificationTokenRepository emailVerificationTokenRepository,
    IEmailVerify emailVerify,
    IFluentEmail fluentEmail,
    IConfiguration configuration) : ICommandHandler<DeleteCustomerAccountCommand>
{
    public async Task<Result> Handle(DeleteCustomerAccountCommand request, CancellationToken cancellationToken)
    {
        //Decode jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        if (string.IsNullOrEmpty(userId))
        {
            var errors = new[] { new Error("id", "Invalid id") };
            return Result.Failure(errors);
        }

        var customer = await customerAccountRepository.GetCustomerById(Ulid.Parse(userId));

        //tạo verification token
        var emailVerificationToken = new EmailVerificationToken
        {
            EmailVerificationTokenId = Ulid.NewUlid(),
            ExpiredDate = DateTime.UtcNow.AddDays(1),
            UserId = Ulid.Parse(userId),
            CreatedDate = DateTime.UtcNow
        };

        await emailVerificationTokenRepository.CreateVerificationToken(emailVerificationToken);
        await unitOfWork.SaveChangesAsync();

        //FIX: Xử lý hard code
        // Gửi mail xác nhận
        var verificationLink = emailVerify.CreateLinkForDeleteCustomerAccount(emailVerificationToken);
        bool emailSent = false;
        int retryCount = 0;
        int maxRetries = 5;

        do
        {
            if (configuration["Environment"] == "Development")
            {
                try
                {
                    await fluentEmail.To(customer.Email).Subject("Nhà hàng Nhum nhum - Xác nhận huỷ tài khoản")
                        .Body($"Vui lòng xác nhận để huỷ tài khoản bằng cách click vào link sau: <a href='{verificationLink}'>Click me</a> <br> Quý khách vui lòng xác nhận trong 24h, sau thời gian này, yêu cầu sẽ tự huỷ.", isHtml: true)
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
                    string toEmail = customer.Email;  // Địa chỉ người nhận
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
                        Subject = "Nhà hàng Nhum nhum - Xác nhận huỷ tài khoản",
                        Body = $"Vui lòng xác nhận để huỷ tài khoản bằng cách click vào link sau: <a href='{verificationLink}'>Click me</a> <br> Quý khách vui lòng xác nhận trong 24h, sau thời gian này, yêu cầu sẽ tự huỷ.",
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


        return Result.Success();
    }
}
