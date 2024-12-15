using System.Net;
using System.Net.Mail;
using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.BookingFeature.Commands.CustomerCreateBooking;


public class CustomerCreateBookingCommandHandler(
    IConfiguration configuration,
    IUnitOfWork unitOfWork,
    IBookingRepository bookingRepository,
    IApplicationDbContext context,
    IFluentEmail fluentEmail) : ICommandHandler<CustomerCreateBookingCommand>
{
    public async Task<Result> Handle(CustomerCreateBookingCommand request, CancellationToken cancellationToken)
    {
        #region ValidateRequest
        var validator = new CustomerCreateBookingCommandValidator(bookingRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }
        #endregion


        #region Kiểm tra user đã tồn tại chưa, Nếu chưa tạo mới
        var isCustomerExist = await context.Customers
            .Include(a => a.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.User.Email == request.Email || a.User.Phone == request.PhoneNumber);

        string userEmail = string.Empty;
        var userId = Ulid.NewUlid();
        var customerId = Ulid.NewUlid();
        if (isCustomerExist == null)
        {
            userEmail = request.Email;
            await context.Users.AddAsync(new User
            {
                UserId = userId,
                Email = request.Email,
                Phone = request.PhoneNumber,
                FirstName = request.FirstName,
                Status = "NotActivated",

                LastName = request.LastName
            });

            await context.Customers.AddAsync(new Customer
            {
                CustomerId = customerId,
                UserId = userId,
                CustomerStatus = "Active",
                CustomerType = "Normal"
            });
        }
        else
        {
            userEmail = isCustomerExist.User.Email;
        }
        #endregion



        #region Tính tiền booking
        TableType[] tableTypes = await context.TableTypes
            .AsNoTracking()
            .Select(a => new TableType
            {
                TableTypeId = a.TableTypeId,
                TablePrice = a.TablePrice,
                TableCapacity = a.TableCapacity
            })
            .ToArrayAsync();

        // Khởi tạo giá booking và biến lưu bàn có sức chứa lớn nhất
        decimal bookingPrice = 0;
        TableType? maxCapacityTable = null;
        Ulid? tableTypeId = null;
        foreach (var item in tableTypes)
        {
            // Tìm bàn nhỏ nhất có thể chứa đủ số lượng khách
            // Nếu tìm thấy, lấy giá của bàn đó và kết thúc vòng lặp
            if (int.Parse(request.NumberOfCustomers.ToString()) <= item.TableCapacity)
            {
                bookingPrice = item.TablePrice;
                tableTypeId = item.TableTypeId;
                break;
            }
            // Song song đó, tìm và lưu lại bàn có sức chứa lớn nhất
            // để dùng làm phương án dự phòng khi không có bàn nào đủ chỗ
            if (maxCapacityTable == null || item.TableCapacity > maxCapacityTable.TableCapacity)
            {
                maxCapacityTable = item;
                tableTypeId = item.TableTypeId;
            }
        }

        // Nếu không tìm được bàn nào đủ chỗ (bookingPrice = 0)
        // Sử dụng giá của bàn có sức chứa lớn nhất
        if (bookingPrice == 0 && maxCapacityTable != null)
        {
            bookingPrice = maxCapacityTable.TablePrice;
        }
        #endregion

        // Kiểm tra số lượng bàn còn lại
        int quantity = await context.Tables
            .AsNoTracking()
            .Where(a => a.TableTypeId == tableTypeId
                && a.ActiveStatus == "Empty")
                .CountAsync();

        if (quantity == 0)
        {
            return Result.Failure(new[] { new Error("Table", "No table available") });
        }



        #region Tạo booking
        var booking = new Booking
        {
            BookId = Ulid.NewUlid(),
            BookingDate = request.BookingDate,
            BookingTime = request.BookingTime,
            BookingPrice = bookingPrice,
            PaymentStatus = "Waiting",
            BookingStatus = "Waiting",
            CreatedDate = DateTime.Now,
            NumberOfCustomers = int.Parse(request.NumberOfCustomers.ToString()),
            CustomerId = isCustomerExist?.CustomerId ?? customerId,
            Note = request.Note
        };
        await context.Bookings.AddAsync(booking);
        #endregion


        await unitOfWork.SaveChangesAsync();

        //NOTE: VNPAY return URL
        #region VnPay
        string paymentUrl = VnPayExtentions.GetVnPayUrl(
            configuration["VNP_RETURNURL_BOOKING"]!,
            (int)bookingPrice,
            booking.BookId.ToString(),
            configuration);
        #endregion

        //FIX: Xử lý hard code
        #region Gửi mail thông báo cho khách hàng
        bool emailSent = false;
        int retryCount = 0;
        int maxRetries = 5;

        do
        {
            if (configuration["Environment"] == "Development")
            {
                try
                {
                    await fluentEmail.To(userEmail).Subject("Nhà hàng Nhum nhum - Thông báo thanh toán phí đặt bàn")
                        .Body($"Quý khách vui lòng thanh toán phí đặt bàn tại đây để hoàn thành thủ tục: <a href='{paymentUrl}'>Click me</a> <br> Mã booking của bạn là: {booking.BookId}", isHtml: true)
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
                        Subject = "Nhà hàng Nhum nhum - Thông báo thanh toán phí đặt bàn",
                        Body = $"Quý khách vui lòng thanh toán phí đặt bàn tại đây để hoàn thành thủ tục: <a href='{paymentUrl}'>Click me</a> <br> Mã booking của bạn là: {booking.BookId}",
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
        #endregion


        return Result.Success();
    }

}

#region Stable Code
// using FluentEmail.Core;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using RestaurantManagement.Application.Abtractions;
// using RestaurantManagement.Application.Data;
// using RestaurantManagement.Application.Extentions;
// using RestaurantManagement.Application.Services;
// using RestaurantManagement.Domain.Entities;
// using RestaurantManagement.Domain.IRepos;
// using RestaurantManagement.Domain.Shared;

// namespace RestaurantManagement.Application.Features.BookingFeature.Commands.CustomerCreateBooking;


// public class CustomerCreateBookingCommandHandler(
//     IConfiguration configuration,
//     IUnitOfWork unitOfWork,
//     IBookingRepository bookingRepository,
//     IApplicationDbContext context,
//     IFluentEmail fluentEmail) : ICommandHandler<CustomerCreateBookingCommand>
// {
//     public async Task<Result> Handle(CustomerCreateBookingCommand request, CancellationToken cancellationToken)
//     {
//         #region ValidateRequest
//         var validator = new CustomerCreateBookingCommandValidator(bookingRepository);
//         Error[]? errors = null;
//         var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
//         if (!isValid)
//         {
//             return Result.Failure(errors!);
//         }
//         #endregion


//         #region Kiểm tra user đã tồn tại chưa, Nếu chưa tạo mới
//         var isCustomerExist = await context.Customers
//             .Include(a => a.User)
//             .AsNoTracking()
//             .FirstOrDefaultAsync(a => a.User.Email == request.Email || a.User.Phone == request.PhoneNumber);

//         string userEmail = string.Empty;
//         var userId = Ulid.NewUlid();
//         var customerId = Ulid.NewUlid();
//         if (isCustomerExist == null)
//         {
//             userEmail = request.Email;
//             await context.Users.AddAsync(new User
//             {
//                 UserId = userId,
//                 Email = request.Email,
//                 Phone = request.PhoneNumber,
//                 FirstName = request.FirstName,
//                 Status = "NotActivated",

//                 LastName = request.LastName
//             });

//             await context.Customers.AddAsync(new Customer
//             {
//                 CustomerId = customerId,
//                 UserId = userId,
//                 CustomerStatus = "Active",
//                 CustomerType = "Normal"
//             });
//         }
//         else
//         {
//             userEmail = isCustomerExist.User.Email;
//         }
//         #endregion



//         #region Tính tiền booking
//         TableType[] tableTypes = await context.TableTypes
//             .AsNoTracking()
//             .Select(a => new TableType
//             {
//                 TableTypeId = a.TableTypeId,
//                 TablePrice = a.TablePrice,
//                 TableCapacity = a.TableCapacity
//             })
//             .ToArrayAsync();

//         // Khởi tạo giá booking và biến lưu bàn có sức chứa lớn nhất
//         decimal bookingPrice = 0;
//         TableType? maxCapacityTable = null;
//         Ulid? tableTypeId = null;
//         foreach (var item in tableTypes)
//         {
//             // Tìm bàn nhỏ nhất có thể chứa đủ số lượng khách
//             // Nếu tìm thấy, lấy giá của bàn đó và kết thúc vòng lặp
//             if (int.Parse(request.NumberOfCustomers.ToString()) <= item.TableCapacity)
//             {
//                 bookingPrice = item.TablePrice;
//                 tableTypeId = item.TableTypeId;
//                 break;
//             }
//             // Song song đó, tìm và lưu lại bàn có sức chứa lớn nhất
//             // để dùng làm phương án dự phòng khi không có bàn nào đủ chỗ
//             if (maxCapacityTable == null || item.TableCapacity > maxCapacityTable.TableCapacity)
//             {
//                 maxCapacityTable = item;
//                 tableTypeId = item.TableTypeId;
//             }
//         }

//         // Nếu không tìm được bàn nào đủ chỗ (bookingPrice = 0)
//         // Sử dụng giá của bàn có sức chứa lớn nhất
//         if (bookingPrice == 0 && maxCapacityTable != null)
//         {
//             bookingPrice = maxCapacityTable.TablePrice;
//         }
//         #endregion

//         // Kiểm tra số lượng bàn còn lại
//         int quantity = await context.Tables
//             .AsNoTracking()
//             .Where(a => a.TableTypeId == tableTypeId
//                 && a.ActiveStatus == "Empty")
//                 .CountAsync();

//         if (quantity == 0)
//         {
//             return Result.Failure(new[] { new Error("Table", "No table available") });
//         }



//         #region Tạo booking
//         var booking = new Booking
//         {
//             BookId = Ulid.NewUlid(),
//             BookingDate = request.BookingDate,
//             BookingTime = request.BookingTime,
//             BookingPrice = bookingPrice,
//             PaymentStatus = "Waiting",
//             BookingStatus = "Waiting",
//             CreatedDate = DateTime.Now,
//             NumberOfCustomers = int.Parse(request.NumberOfCustomers.ToString()),
//             CustomerId = isCustomerExist?.CustomerId ?? customerId,
//             Note = request.Note
//         };
//         await context.Bookings.AddAsync(booking);
//         #endregion


//         await unitOfWork.SaveChangesAsync();
//         //NOTE: VNPAY return URL

//         #region VnPay
//         //Get Config Info
//         string vnp_Returnurl = "https://localhost:7057/api/booking/ReturnUrl"; //URL nhan ket qua tra ve 
//         string vnp_Url = configuration["VNP_URL"]!; //URL thanh toan cua VNPAY 
//         string vnp_TmnCode = configuration["VNP_TMNCODE"]!; //Ma định danh merchant kết nối (Terminal Id)
//         string vnp_HashSecret = configuration["VNP_TMNCODE"]!; //Secret Key



//         //Build URL for VNPAY
//         VnPayLibrary vnpay = new VnPayLibrary();

//         vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
//         vnpay.AddRequestData("vnp_Command", "pay");
//         vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
//         vnpay.AddRequestData("vnp_Amount", ((int)bookingPrice / 2 * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
//         vnpay.AddRequestData("vnp_BankCode", "");
//         vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
//         vnpay.AddRequestData("vnp_CurrCode", "VND");
//         vnpay.AddRequestData("vnp_IpAddr", ":1");
//         vnpay.AddRequestData("vnp_Locale", "vn");
//         vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + booking.BookId);
//         vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

//         vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
//         vnpay.AddRequestData("vnp_TxnRef", booking.BookId.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

//         string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
//         #endregion

//         #region Gửi mail thông báo cho khách hàng
//         bool emailSent = false;
//         int retryCount = 0;
//         int maxRetries = 5;

//         do
//         {
//             try
//             {
//                 await fluentEmail.To(userEmail)
//                     .Subject("Nhà hàng Nhum nhum - Thông báo thanh toán phí đặt bàn")
//                     .Body($"Quý khách vui lòng thanh toán phí đặt bàn tại đây để hoàn thành thủ tục: <a href='{paymentUrl}'>Click me</a> <br> Mã booking của bạn là: {booking.BookId}", isHtml: true)
//                     .SendAsync();
//                 emailSent = true;
//             }
//             catch
//             {
//                 retryCount++;
//                 if (retryCount >= maxRetries)
//                 {
//                     return Result.Failure(new[] { new Error("Email", "Failed to send email") });
//                 }
//             }
//         }
//         while (!emailSent && retryCount < maxRetries);
//         #endregion


//         return Result.Success();
//     }

// }
#endregion