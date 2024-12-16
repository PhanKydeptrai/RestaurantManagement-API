using System.Net;
using System.Net.Mail;
using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.BookingFeature.Commands.TableArrangement;

public class TableArrangementCommandHandler(
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork,
    ITableRepository tableRepository,
    IApplicationDbContext context,
    IFluentEmail fluentEmail,
    IConfiguration configuration) : ICommandHandler<TableArrangementCommand>
{
    public async Task<Result> Handle(TableArrangementCommand request, CancellationToken cancellationToken)
    {
        //Validate request
        var validator = new TableArrangementCommandValidator(bookingRepository, tableRepository, context);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        // var bookingId = context.InstanceToValidate.BookingId;
        var recentBooking = await context.Bookings.FindAsync(Ulid.Parse(request.BookingId));
        //Kiểm tra xem bàn đã được book chưa
        var bookingInfomation = await context.Bookings.Include(a => a.BookingDetails)
            .Where(
                a => a.BookingStatus == "Seated"
                &&
                a.BookingDetails.FirstOrDefault().TableId == int.Parse(request.TableId.ToString()!)) //đã xếp bàn
            .ToListAsync();

        foreach (var info in bookingInfomation)
        {
            //So sánh ngày book hiện tại với ngaỳ book của booking đã xếp bàn
            //Nếu cùng ngày thì kiểm tra giờ
            if (info.BookingDate.ToString("dd/MM/yyyy") == recentBooking.BookingDate.ToString("dd/MM/yyyy"))
            {
                if (recentBooking.BookingTime < info.BookingTime.AddHours(+4) || recentBooking.BookingTime > info.BookingTime.AddHours(-2)) //Nếu nằm trong khung giờ thì sẽ không xếp bàn
                {
                    return Result.Failure(new[] { new Error("Table", "Table is not available") });
                }
            }
        }

        var userEmail = await context.Bookings
            .AsNoTracking()
            .Include(a => a.Customer)
            .ThenInclude(a => a.User)
            .Where(a => a.BookId == Ulid.Parse(request.BookingId))
            .Select(a => a.Customer.User.Email)
            .FirstOrDefaultAsync();

        //Tạo một booking detail
        var bookingDetail = new BookingDetail
        {
            BookId = Ulid.Parse(request.BookingId),
            BookingDetailId = Ulid.NewUlid(),
            TableId = int.Parse(request.TableId.ToString())
        };

        //Cập nhật trạng thái đã xếp bàn cho booking
        await bookingRepository.UpdateBookingStatus(Ulid.Parse(request.BookingId));

        await context.BookingDetails.AddAsync(bookingDetail);

        //Cập nhật active status của bàn => Status này sẽ được cập nhật bằng background service
        // await tableRepository.UpdateActiveStatus(int.Parse(request.TableId.ToString()), "Booked");



        //thông báo cho người dùng
        var bookingInfo = await bookingRepository.GetBookingResponseById(Ulid.Parse(request.BookingId));
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
                    await fluentEmail.To(userEmail)
                        .Subject("Nhà hàng Nhum nhum - Thông báo thông tin đặt bàn")
                        .Body($"Nhà hàng đã xác nhận được thông tin đặt bàn của bạn.<br> Đây là thông tin đặt bàn của bạn: <br> Mã Booking của bạn là: {bookingInfo.BookId} <br>Tên: {bookingInfo.LastName + " " + bookingInfo.FirstName} <br> Ngày:{bookingInfo.BookingDate}<br> Thời gian: {bookingInfo.BookingTime} <br> Email: {bookingInfo.Email} <br> Số điện thoại:{bookingInfo.Phone} <br> Số bàn của bạn là: {bookingDetail.TableId} ", isHtml: true)
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
                    //Gửi mail
                    #region Send Email using Gmail SMTP
                    // Thông tin đăng nhập và cài đặt máy chủ SMTP
                    string fromEmail = "nhumnhumrestaurant@gmail.com"; // Địa chỉ Gmail của bạn
                    string toEmail = userEmail;  // Địa chỉ người nhận
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
                        Subject = "Nhà hàng Nhum nhum - Thông báo thông tin đặt bàn",
                        Body = $"Nhà hàng đã xác nhận được thông tin đặt bàn của bạn.<br> Đây là thông tin đặt bàn của bạn: <br> Mã Booking của bạn là: {bookingInfo.BookId} <br>Tên: {bookingInfo.LastName + " " + bookingInfo.FirstName} <br> Ngày:{bookingInfo.BookingDate}<br> Thời gian: {bookingInfo.BookingTime} <br> Email: {bookingInfo.Email} <br> Số điện thoại:{bookingInfo.Phone} <br> Số bàn của bạn là: {bookingDetail.TableId} ",
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
        //TODO: Log

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
#region Stable code
// public class TableArrangementCommandHandler(
//     IBookingRepository bookingRepository,
//     IUnitOfWork unitOfWork,
//     ITableRepository tableRepository,
//     IApplicationDbContext context,
//     IFluentEmail fluentEmail) : ICommandHandler<TableArrangementCommand>
// {
//     public async Task<Result> Handle(TableArrangementCommand request, CancellationToken cancellationToken)
//     {

//         var validator = new TableArrangementCommandValidator(bookingRepository, tableRepository);
//         if (!ValidateRequest.RequestValidator(validator, request, out var errors))
//         {
//             return Result.Failure(errors);
//         }

//         //Cập nhật trạng thái đã xếp bàn cho booking
//         await bookingRepository.UpdateBookingStatus(Ulid.Parse(request.BookingId));
//         var userEmail = await context.Bookings
//             .Include(a => a.Customer)
//             .ThenInclude(a => a.User)
//             .Where(a => a.BookId == Ulid.Parse(request.BookingId))
//             .Select(a => a.Customer.User.Email)
//             .FirstOrDefaultAsync();

//         //Tạo một booking detail
//         var bookingDetail = new BookingDetail
//         {
//             BookId = Ulid.Parse(request.BookingId),
//             BookingDetailId = Ulid.NewUlid(),
//             TableId = int.Parse(request.TableId)
//         };

//         await context.BookingDetails.AddAsync(bookingDetail);
//         //Cập nhật active status của bàn
//         await tableRepository.UpdateActiveStatus(int.Parse(request.TableId), "Booked");
//         await unitOfWork.SaveChangesAsync();

//         //thông báo cho người dùng
//         var bookingInfo = await bookingRepository.GetBookingResponseById(Ulid.Parse(request.BookingId));
//         await fluentEmail.To(userEmail)
//             .Subject("Nhà hàng Nhum nhum - Thông báo thông tin đặt bàn")
//             .Body($"Nhà hàng đã xác nhận được thông tin đặt bàn của bạn.<br> Đây là thông tin đặt bàn của bạn: <br> Mã Booking của bạn là: {bookingInfo.BookId} <br>Tên: {bookingInfo.LastName + " " + bookingInfo.FirstName} <br> Ngày:{bookingInfo.BookingDate}<br> Thời gian: {bookingInfo.BookingTime} <br> Email: {bookingInfo.Email} <br> Số điện thoại:{bookingInfo.Phone} <br> Số bàn của bạn là: {bookingDetail.TableId} " , isHtml: true)
//             .SendAsync();

//         return Result.Success();
//     }
// }
#endregion
