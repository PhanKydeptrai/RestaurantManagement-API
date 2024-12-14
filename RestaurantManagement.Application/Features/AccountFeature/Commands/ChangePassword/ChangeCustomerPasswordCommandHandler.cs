using System.Net;
using System.Net.Mail;
using FluentEmail.Core;
using NETCore.Encrypt;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ChangeCustomerPassword;

public class ChangeCustomerPasswordCommandHandler(

    IUserRepository userRepository,
    IFluentEmail fluentEmail,
    IEmailVerificationTokenRepository emailVerificationTokenRepository,
    IEmailVerify emailVerify,
    IUnitOfWork unitOfWork) : ICommandHandler<ChangePasswordCommand>
{
    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        //Validate request
        var validator = new ChangeCustomerPasswordCommandValidator();
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        //Lấy token
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Kiểm tra mật khẩu cũ
        var user = await userRepository.GetUserById(Ulid.Parse(userId));
        string encryptPass = EncryptProvider.Sha256(request.oldPass);
        if (encryptPass != user.Password)
        {
            return Result.Failure(new[] { new Error("OldPassword", "Mật khẩu cũ không đúng") });
        }
        //Tạo token xác thực 
        var token = new EmailVerificationToken
        {
            CreatedDate = DateTime.Now,
            ExpiredDate = DateTime.Now.AddMinutes(5),
            EmailVerificationTokenId = Ulid.NewUlid(),
            UserId = Ulid.Parse(userId),
            Temporary = EncryptProvider.Sha256(request.newPass)
        };
        await emailVerificationTokenRepository.CreateVerificationToken(token);

        string verificationLink = emailVerify.CreateLinkForChangePass(token);
        //FIX: Xử lý hard code 
        //Gửi mail
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
                    Body = $"Vui lòng xác nhận để thay đôi mật khẩu bằng cách click vào link sau: <a href='{verificationLink}'>Click me</a>",
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


        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
