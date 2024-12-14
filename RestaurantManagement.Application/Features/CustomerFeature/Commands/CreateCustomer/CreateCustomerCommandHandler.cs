using System.Net;
using System.Net.Mail;
using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using NETCore.Encrypt;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CustomerFeature.Commands.CreateCustomer;

public class CreateCustomerCommandHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    IApplicationDbContext context,
    IFluentEmail fluentEmail,
    IEmailVerify emailVerify) : ICommandHandler<CreateCustomerCommand>
{

    public async Task<Result> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        //Validate request
        var validator = new CreateCustomerCommandValidator(customerRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        var normalCustomer = await context.Customers.Include(a => a.User)
            .FirstOrDefaultAsync(a => a.User.Email == request.Email || a.User.Phone == request.Phone);

        string randomPassword = RandomStringGenerator.GenerateRandomString(8);
        string randomEncryptPassword = EncryptProvider.Sha256(randomPassword);
        if (normalCustomer != null && normalCustomer.CustomerType != "Subscriber")
        {
            //Update tài khoản
            normalCustomer.CustomerType = "Subscriber";
            normalCustomer.CustomerStatus = "Active";
            normalCustomer.User.Status = "NotActivated";
            normalCustomer.User.FirstName = request.FirstName;
            normalCustomer.User.LastName = request.LastName;
            normalCustomer.User.Gender = request.Gender;
            normalCustomer.User.Password = randomEncryptPassword;
            await unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        //Create User
        var user = new User
        {
            UserId = Ulid.NewUlid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            ImageUrl = string.Empty,
            Phone = request.Phone,
            Password = randomEncryptPassword,
            Gender = request.Gender,
            Status = "NotActivated"
        };
        await userRepository.CreateUser(user);
        // Create Customer
        var customer = new Customer
        {
            CustomerId = Ulid.NewUlid(),
            UserId = user.UserId,
            CustomerStatus = "Active",
            CustomerType = "Subscriber",
        };
        await customerRepository.CreateCustomer(customer);

        //End

        // Create email verification token
        var emailVerificationToken = new EmailVerificationToken
        {
            EmailVerificationTokenId = Ulid.NewUlid(),
            ExpiredDate = DateTime.UtcNow.AddDays(1),
            UserId = user.UserId,
            CreatedDate = DateTime.UtcNow
        };
        //End
        
        //FIX: Xử lý hard code
        //gửi mail kích hoạt tài khoản

        var verificationLink = emailVerify.Create(emailVerificationToken);

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
                string toEmail = request.Email;  // Địa chỉ người nhận
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
                    Subject = "Nhà hàng Nhum nhum - Thông báo kích hoạt tài khoản",
                    Body = $"Vui lòng kích hoạt tài khoản bằng cách click vào link sau: <a href='{verificationLink}'>Click me</a> <br> Đây là mật khẩu của bạn: {randomPassword}",
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

        #region Decode jwt and system log
        //Decode jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        // Create System Log
        await context.CustomerLogs.AddAsync(new CustomerLog
        {
            CustomerLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetails = $"Tạo tài khoản cho khách {request.FirstName + " " + request.LastName + "." + $"ID: {user.UserId}"}",
            UserId = Ulid.Parse(userId)
        });
        #endregion


        await context.EmailVerificationTokens.AddAsync(emailVerificationToken);
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
