using System.Net;
using System.Net.Mail;
using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Quartz;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Infrastructure.BackgroundJob;

public class VoucherBackgroundJob : IJob
{
    private readonly IApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFluentEmail _fluentEmail;

    public VoucherBackgroundJob(
        IApplicationDbContext context,
        IUnitOfWork unitOfWork,
        IFluentEmail fluentEmail,
        IConfiguration configuration)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _fluentEmail = fluentEmail;
        _configuration = configuration;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        //Kiểm tra toàn bộ khách hàng. Nếu có voucher hết hạn thì xóa voucher đó và gửi mail thông báo

        //Kiểm tra voucher hết hạn
        Voucher[]? expiredVouchers = await _context.Vouchers
            .Where(v => v.ExpiredDate < DateTime.Now)
            .ToArrayAsync();

        //Cập nhật voucher hết hạn
        foreach (var voucher in expiredVouchers)
        {
            voucher.Status = "InActive";

            CustomerVoucher[]? customerVouchers = await _context.CustomerVouchers
                .Include(a => a.Voucher)
                .Include(a => a.Customer)
                .ThenInclude(a => a.User)
                .Where(a => a.VoucherId == voucher.VoucherId)
                .ToArrayAsync();
            foreach (var customerVoucher in customerVouchers)
            {
                //Gửi mail thông báo cho khách hàng
                bool emailSent = false;
                int retryCount = 0;
                int maxRetries = 5;

                do
                {
                    if (_configuration["Environment"] == "Development")
                    {
                        try
                        {
                            await _fluentEmail.To(customerVoucher.Customer.User.Email)
                                .Subject("Nhà hàng Nhum nhum - Voucher hết hạn 😢")
                                .Body($"Nhà hàng Nhum Nhum xin thông báo <br> Voucher {customerVoucher.Voucher.VoucherCode} của bạn đã hết hạn.", isHtml: true)
                                .SendAsync();

                            emailSent = true;
                        }
                        catch
                        {
                            retryCount++;
                            // if (retryCount >= maxRetries)
                            // {
                            //     return Result.Failure(new[] { new Error("Email", "Failed to send email") });
                            // }
                        }
                    }
                    else
                    {
                        try
                        {
                            #region Send Email using Gmail SMTP
                            // Thông tin đăng nhập và cài đặt máy chủ SMTP
                            string fromEmail = "nhumnhumrestaurant@gmail.com"; // Địa chỉ Gmail của bạn
                            string toEmail = customerVoucher.Customer.User.Email;  // Địa chỉ người nhận
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
                                Subject = "Nhà hàng Nhum nhum - Voucher hết hạn 😢",
                                Body = $"", // Nội dung email
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
                            // if (retryCount >= maxRetries)
                            // {
                            //     return Result.Failure(new[] { new Error("Email", "Failed to send email") });
                            // }
                        }
                    }

                }
                while (!emailSent && retryCount < maxRetries);
            }
        }

        //Cập nhật thay đổi cho voucher        

        await _unitOfWork.SaveChangesAsync();

        Console.WriteLine("Voucher background job is running");
    }
}
