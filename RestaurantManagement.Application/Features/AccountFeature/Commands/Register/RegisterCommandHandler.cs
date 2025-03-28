﻿using System.Net;
using System.Net.Mail;
using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NETCore.Encrypt;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.Register;

public class RegisterCommandHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    IApplicationDbContext context,
    IFluentEmail fluentEmail,
    IEmailVerify emailVerify,
    IConfiguration configuration) : ICommandHandler<RegisterCommand>
{
    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        //Validate request
        var validator = new RegisterCommandValidator(customerRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        //REFACTOR:
        // 1. Sử dụng repository
        // 2. Tối ưu trường hợp khách đăng ký bị chéo thông tin giữa hai cặp tài khoản thường.
        var normalCustomer = await context.Customers.Include(a => a.User)
            .FirstOrDefaultAsync(a => a.User.Email == request.Email || a.User.Phone == request.Phone);

        if(normalCustomer != null && normalCustomer.CustomerType == "Subscriber" && string.IsNullOrEmpty(normalCustomer.User.Password))
        {
            //Update tài khoản
            normalCustomer.CustomerType = "Subscriber";
            normalCustomer.CustomerStatus = "Active";
            // normalCustomer.User.Status = "NotActivated";
            normalCustomer.User.FirstName = request.FirstName;
            normalCustomer.User.LastName = request.LastName;
            normalCustomer.User.Gender = request.Gender;
            normalCustomer.User.Password = EncryptProvider.Sha256(request.Password);
            
            await unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        if (normalCustomer != null && normalCustomer.CustomerType != "Subscriber")
        {
            //Update tài khoản
            normalCustomer.CustomerType = "Subscriber";
            normalCustomer.CustomerStatus = "Active";
            normalCustomer.User.Status = "NotActivated";
            normalCustomer.User.FirstName = request.FirstName;
            normalCustomer.User.LastName = request.LastName;
            normalCustomer.User.Gender = request.Gender;
            normalCustomer.User.Password = EncryptProvider.Sha256(request.Password);
            //Create email verification token
            var emailverificationToken = new EmailVerificationToken
            {
                EmailVerificationTokenId = Ulid.NewUlid(),
                ExpiredDate = DateTime.UtcNow.AddDays(1),
                UserId = normalCustomer.UserId,
                CreatedDate = DateTime.UtcNow
            };

            await unitOfWork.SaveChangesAsync();

            //FIX: Xử lý hard code
            //gửi mail kích hoạt tài khoản
            var verifiCationLink = emailVerify.Create(emailverificationToken);
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
                        await fluentEmail.To(normalCustomer.User.Email).Subject("Nhà hàng Nhum nhum - Thông báo kích hoạt tài khoản")
                            .Body($"Vui lòng kích hoạt tài khoản bằng cách click vào link sau: <a href='{verifiCationLink}'>Click me</a>", isHtml: true)
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
                        string toEmail = normalCustomer.User.Email;  // Địa chỉ người nhận
                        string password = "ekgh lntd brrv bdyj";  

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
                            Body = $"Vui lòng kích hoạt tài khoản bằng cách click vào link sau: <a href='{verifiCationLink}'>Click me</a>",
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

        //create user
        var user = new User
        {
            UserId = Ulid.NewUlid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            ImageUrl = string.Empty,
            Phone = request.Phone,
            Password = EncryptProvider.Sha256(request.Password),
            Gender = request.Gender,
            Status = "NotActivated"
        };

        var customer = new Customer
        {
            CustomerId = Ulid.NewUlid(),
            UserId = user.UserId,
            CustomerStatus = "Active",
            CustomerType = "Subscriber",
        };

        //Create email verification token
        var emailVerificationToken = new EmailVerificationToken
        {
            EmailVerificationTokenId = Ulid.NewUlid(),
            ExpiredDate = DateTime.UtcNow.AddDays(1),
            UserId = user.UserId,
            CreatedDate = DateTime.UtcNow
        };
        await context.EmailVerificationTokens.AddAsync(emailVerificationToken);
        await userRepository.CreateUser(user);
        await customerRepository.CreateCustomer(customer);

        //FIX: Xử lý hard code
        //gửi mail kích hoạt tài khoản
        var verificationLink = emailVerify.Create(emailVerificationToken);

        bool emailSent2 = false;
        int retryCount2 = 0;
        int maxRetries2 = 5;

        do
        {
            if (configuration["Environment"] == "Development")
            {
                try
                {
                    await fluentEmail.To(user.Email).Subject("Nhà hàng Nhum nhum - Thông báo kích hoạt tài khoản")
                        .Body($"Vui lòng kích hoạt tài khoản bằng cách click vào link sau: <a href='{verificationLink}'>Click me</a>", isHtml: true)
                        .SendAsync();

                    emailSent2 = true;
                }
                catch
                {
                    retryCount2++;
                    if (retryCount2 >= maxRetries2)
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
                        Subject = "Nhà hàng Nhum nhum - Thông báo kích hoạt tài khoản",
                        Body = $"Vui lòng kích hoạt tài khoản bằng cách click vào link sau: <a href='{verificationLink}'>Click me</a>",
                        IsBodyHtml = true // Nếu muốn gửi email ở định dạng HTML
                    };
    
                    mailMessage.To.Add(toEmail);
    
                    // Gửi email
                    smtpClient.Send(mailMessage);
                    #endregion
                    emailSent2 = true;
                }
                catch
                {
                    retryCount2++;
                    if (retryCount2 >= maxRetries2)
                    {
                        return Result.Failure(new[] { new Error("Email", "Failed to send email") });
                    }
                }
            }
        }
        while (!emailSent2 && retryCount2 < maxRetries2);


        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
