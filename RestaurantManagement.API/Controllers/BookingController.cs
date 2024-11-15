using FluentEmail.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Features.BookingFeature.Commands.CancelCreateBooking;
using RestaurantManagement.Application.Features.BookingFeature.Commands.CustomerCreateBooking;
using RestaurantManagement.Application.Features.BookingFeature.Commands.SubscriberCreateBooking;
using RestaurantManagement.Application.Features.BookingFeature.Commands.TableArrangement;
using RestaurantManagement.Application.Features.BookingFeature.Queries.GetAllBooking;
using RestaurantManagement.Application.Features.BookingFeature.Queries.GetBookingByBookingId;
using RestaurantManagement.Application.Features.BookingFeature.Queries.GetBookingByUserId;
using RestaurantManagement.Domain.DTOs.PaymentDtos;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.API.Controllers;

public class BookingController : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("api/booking").WithTags("Booking").DisableAntiforgery().RequireRateLimiting("AntiSpam");

        endpoints.MapGet("", async (
            [FromQuery] string? filterBookingStatus,
            [FromQuery] string? filterPaymentStatus,
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            ISender sender) =>
        {
            var result = await sender.Send(new GetAllBookingQuery(
                filterBookingStatus,
                filterPaymentStatus,
                searchTerm,
                sortColumn,
                sortOrder,
                page,
                pageSize));

            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        });

        //Lấy thông tin booking theo booking id
        endpoints.MapGet("{id}", async (
            string id,
            ISender sender) =>
        {

            var result = await sender.Send(new GetBookingByBookingIdQuery(id));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.NoContent();
        });

        //Lấy thông tin booking theo user id
        endpoints.MapGet("user/{id}", async (
            string id,
            ISender sender) =>
        {
            var result = await sender.Send(new GetBookingByUserIdQuery(id));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        });

        //Khách đặt bàn không login
        endpoints.MapPost("", async (
            [FromBody] CustomerCreateBookingCommand command,
            ISender sender) =>
        {

            var result = await sender.Send(command);
            if (result.IsSuccess)
            {

                return Results.Ok(result);

            }
            return Results.BadRequest(result);
        });

        //Khách đặt bàn đã login
        endpoints.MapPost("subcriber", async (
            [FromBody] SubscriberCreateBookingRequest command,
            ISender sender,
            HttpContext httpContext,
            IJwtProvider jwtProvider) =>
        {

            //lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);

            var result = await sender.Send(new SubscriberCreateBookingCommand(
                command.BookingDate,
                command.BookingTime,
                command.NumberOfCustomers,
                command.Note,
                token
            ));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).RequireAuthorization();

        //Xếp bàn cho khách 
        endpoints.MapPost("table-arrange/{BookingId}", async (
            string BookingId,
            [FromBody] TableArrangementRequest command,
            ISender sender) =>
        {
            var result = await sender.Send(new TableArrangementCommand(BookingId, command.TableId));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).RequireAuthorization();

        //Hủy đặt bàn
        endpoints.MapDelete("{id}", async (
            string id,
            ISender sender) =>
        {
            var result = await sender.Send(new CancelBookingCommand(id));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);

        }).RequireAuthorization();

        //Trả về url thanh toán
        endpoints.MapGet("ReturnUrl", async (
            [FromQuery(Name = "vnp_Amount")] string vnp_Amount,
            [FromQuery(Name = "vnp_BankCode")] string vnp_BankCode,
            [FromQuery(Name = "vnp_OrderInfo")] string vnp_OrderInfo,
            [FromQuery(Name = "vnp_ResponseCode")] string vnp_ResponseCode,
            [FromQuery(Name = "vnp_TmnCode")] string vnp_TmnCode,
            [FromQuery(Name = "vnp_TransactionNo")] string vnp_TransactionNo,
            [FromQuery(Name = "vnp_TxnRef")] string vnp_TxnRef,
            [FromQuery(Name = "vnp_SecureHash")] string vnp_SecureHash,
            IUnitOfWork unitOfWork,
            IApplicationDbContext _context,
            IFluentEmail fluentEmail) =>
        {
            var model = new VnPayReturnModel
            {
                vnp_Amount = vnp_Amount,
                vnp_BankCode = vnp_BankCode,
                vnp_OrderInfo = vnp_OrderInfo,
                vnp_ResponseCode = vnp_ResponseCode,
                vnp_TmnCode = vnp_TmnCode,
                vnp_TransactionNo = vnp_TransactionNo,
                vnp_TxnRef = vnp_TxnRef,
                vnp_SecureHash = vnp_SecureHash
            };

            #region Xác thực dữ liệu trả về từ VNPAY
            // bool isValid = ValidateVnPayReturn(model);
            // if (!isValid)
            // {
            //     return Results.BadRequest("Invalid VNPAY return data");
            // }
            #endregion


            // Cập nhật thông tin trong cơ sở dữ liệu
            var booking = await _context.Bookings.Include(a => a.Customer)
                .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(b => b.BookId == Ulid.Parse(model.vnp_TxnRef));

            if (booking == null)
            {
                return Results.NotFound("Booking not found");
            }

            if (booking.PaymentStatus == "Paid")
            {
                return Results.Ok("Payment is paid!");
            }

            booking.PaymentStatus = model.vnp_ResponseCode == "00" ? "Paid" : "Failed";
            await unitOfWork.SaveChangesAsync();

            // Gửi email thông báo
            bool emailSent = false;
            int retryCount = 0;
            int maxRetries = 5;

            do
            {
                try
                {
                    await fluentEmail.To(booking.Customer.User.Email)
                        .Subject("Nhà hàng Nhum Nhum - Thông báo thanh toán thành công")
                        .Body($"Quý khách đã thanh toán thành công. <br> Quý khách vui lòng chú ý email để nhận thông tin khi được xếp bàn. <br> Nhà hàng Nhum Nhum xin chân thành cảm ơn.", isHtml: true)
                        .SendAsync();

                    emailSent = true;
                }
                catch
                {
                    retryCount++;
                    if (retryCount >= maxRetries)
                    {
                        return Results.BadRequest("Failed to send email");
                    }
                }
            }
            while (!emailSent && retryCount < maxRetries);

            return Results.Ok("Payment Success!");
        });

    }

    #region Validate VnPay Return
    // private bool ValidateVnPayReturn(VnPayReturnModel model)
    // {
    //     VnPayLibrary vnpay = new VnPayLibrary();
    //     vnpay.AddResponseData("vnp_Amount", model.vnp_Amount);
    //     vnpay.AddResponseData("vnp_BankCode", model.vnp_BankCode);
    //     vnpay.AddResponseData("vnp_OrderInfo", model.vnp_OrderInfo);
    //     vnpay.AddResponseData("vnp_ResponseCode", model.vnp_ResponseCode);
    //     vnpay.AddResponseData("vnp_TmnCode", model.vnp_TmnCode);
    //     vnpay.AddResponseData("vnp_TransactionNo", model.vnp_TransactionNo);
    //     vnpay.AddResponseData("vnp_TxnRef", model.vnp_TxnRef);
    //     vnpay.AddResponseData("vnp_SecureHash", model.vnp_SecureHash);
    //     string vnp_HashSecret = "VJJDQOWMKEA13EFEMV1VGY2A17KDM5Z0"; // Secret Key
    //     return vnpay.ValidateSignature(model.vnp_SecureHash, vnp_HashSecret);
    // }
    #endregion

}


