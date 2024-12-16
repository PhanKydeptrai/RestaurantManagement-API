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

public class SendVoucherToCustomer : IJob
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFluentEmail _fluentEmail;
    private readonly IConfiguration _configuration;
    private readonly IApplicationDbContext _context;

    public SendVoucherToCustomer(
        IUnitOfWork unitOfWork,
        IApplicationDbContext context,
        IFluentEmail fluentEmail,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _context = context;
        _fluentEmail = fluentEmail;
        _configuration = configuration;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        //Kiểm tra xem có voucher nào cần gửi không
        Customer[]? customers = await _context.Customers.Include(a => a.User).ToArrayAsync();
        Voucher[]? vouchers = await _context.Vouchers.ToArrayAsync();
        foreach (var customer in customers)
        {
            //Gửi voucher
            var billQuery = _context.Bills
                .AsNoTracking()
                .Where(a => a.Booking.Customer.UserId == customer.UserId)
                .Include(a => a.Booking)
                .ThenInclude(a => a.Customer.User)
                .Include(a => a.Order)
                .ThenInclude(a => a.OrderDetails)
                .ThenInclude(a => a.Meal)
                .ToArray();

            var a = billQuery.Sum(a => a.Total);

            foreach (var voucher in vouchers)
            {
                if (a > voucher.VoucherConditions)
                {
                    _context.CustomerVouchers.Add(new CustomerVoucher
                    {
                        CustomerVoucherId = Ulid.NewUlid(),
                        CustomerId = customer.CustomerId,
                        VoucherId = voucher.VoucherId,
                        Quantity = 1
                    });

                    bool emailSent = false;
                    int retryCount = 0;
                    int maxRetries = 5;

                    do
                    {
                        if (_configuration["Environment"] == "Development")
                        {
                            try
                            {
                                await _fluentEmail.To(customer.User.Email).Subject("Nhà hàng Nhum nhum - Gửi tặng voucher")
                                    .Body($"Nhà hàng Nhum Nhum xin gửi tặng voucher {voucher.VoucherCode} <br>Quý khách có thể sử dụng khi thanh toán tại nhà hàng. <br>Để biết thêm thông tin của voucher, quý khách có thể tra cứu tại <a href='https://nhumnhumrestaurant.vercel.app'>trang web</a> của nhà hàng.", isHtml: true)
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
                                string toEmail = customer.User.Email;  // Địa chỉ người nhận
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
                                    Subject = "Nhà hàng Nhum nhum - Gửi tặng voucher",
                                    Body = $"Nhà hàng Nhum Nhum xin gửi tặng voucher {voucher.VoucherCode} <br>Quý khách có thể sử dụng khi thanh toán tại nhà hàng. <br>Để biết thêm thông tin của voucher, quý khách có thể tra cứu tại <a href='https://nhumnhumrestaurant.vercel.app'>trang web</a> của nhà hàng.",
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

        }

        await _unitOfWork.SaveChangesAsync();
    }
}
