using FluentEmail.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.API.Authentication;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Features.OrderFeature.Commands.AddMealToOrder;
using RestaurantManagement.Application.Features.OrderFeature.Commands.DeleteMealFromOrder;
using RestaurantManagement.Application.Features.OrderFeature.Commands.MakePayment;
using RestaurantManagement.Application.Features.OrderFeature.Commands.PayOrder;
using RestaurantManagement.Application.Features.OrderFeature.Commands.PayOrderWithVnPay;
using RestaurantManagement.Application.Features.OrderFeature.Commands.RemoveTransaction;
using RestaurantManagement.Application.Features.OrderFeature.Commands.UpdateMealInOrder;
using RestaurantManagement.Application.Features.OrderFeature.Queries.GetAllOrder;
using RestaurantManagement.Application.Features.OrderFeature.Queries.GetMakePaymentInformation;
using RestaurantManagement.Application.Features.OrderFeature.Queries.GetOrderById;
using RestaurantManagement.Domain.DTOs.PaymentDtos;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.API.Controllers;

public class OrderController : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("api/orders").WithTags("Orders").DisableAntiforgery();

        //Create order
        endpoints.MapPost("{id}", async (
            string id,
            [FromBody] AddMealToOrderRequest command,
            ISender sender,
            HttpContext httpContext,
            IJwtProvider jwtProvider) =>
        {
            // Lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);
            var result = await sender.Send(new AddMealToOrderCommand(
                id,
                command.MealId,
                command.Quantity,
                token
            ));

            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            return Results.Ok(result);
        })
        .RequireAuthorization()
        .RequireRateLimiting("AntiSpamAddMealToOrderCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        endpoints.MapPut("{id}", async (
            string id,
            [FromBody] UpdateMealInOrderRequest command,
            ISender sender,
            HttpContext httpContext,
            IJwtProvider jwtProvider) =>
        {
            var result = await sender.Send(new UpdateMealInOrderCommand(id, command.Quantity));
            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            return Results.Ok(result);
        })
        .RequireAuthorization()
        .RequireRateLimiting("AntiSpamUpdateMealInOrderCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();


        endpoints.MapDelete("{id}", async (
            string id,
            ISender sender,
            HttpContext httpContext,
            IJwtProvider jwtProvider) =>
        {
            var result = await sender.Send(new DeleleMealFromOrderCommand(id)); // Đã sửa lỗi chính tả ở đây
            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            return Results.Ok(result);
        })
        .RequireAuthorization()
        .RequireRateLimiting("AntiSpamDeleteMealFromOrderCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        //Get by id
        endpoints.MapGet("{id}", async (
            int id,
            ISender sender) =>
        {
            var result = await sender.Send(new GetOrderByIdQuery(id));
            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            return Results.Ok(result);
        }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        endpoints.MapPut("make-payment/{id}", async (
            string id, // Id bàn
            [FromBody] MakePaymentRequest request,
            ISender sender) =>
        {
            var result = await sender.Send(new MakePaymentCommand(id, request.voucherCode, request.phoneNumber));
            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            return Results.Ok(result);
        }).RequireAuthorization()
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        #region New payorder
        //Pay order
        endpoints.MapPut("pay/{id}",
        async (
            string id, // Id bàn
            ISender sender) =>
        {
            var result = await sender.Send(new PayOrderCommand(id));
            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            return Results.Ok(result);
        })
        .RequireAuthorization()
        .RequireRateLimiting("AntiSpamPayOrderCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();
        #endregion

        //Get all order
        endpoints.MapGet("",
        async (
            [FromQuery] string? filterUserId,
            [FromQuery] string? filterTableId,
            [FromQuery] string? filterPaymentStatus,
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            ISender sender) =>
        {
            var result = await sender.Send(new GetAllOrderQuery(
                filterUserId,
                filterTableId,
                filterPaymentStatus,
                searchTerm,
                sortColumn,
                sortOrder,
                page,
                pageSize));

            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            return Results.Ok(result);
        }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        endpoints.MapGet("make-payment-information/{id}", async (string id, ISender sender) =>
        {
            var result = await sender.Send(new GetMakePaymentInformationQuery(id));

            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            return Results.Ok(result);
        }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();


        endpoints.MapGet("vn-pay/{id}", async (
            string id,
            ISender sender) =>
        {
            var result = await sender.Send(new PayOrderWithVnPayCommand(id));
            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            Console.WriteLine(result.Value);

            return Results.Redirect(result.Value);
        });


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
            IApplicationDbContext context,
            IFluentEmail fluentEmail,
            IConfiguration configuration) =>
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
            var transaction = await context.OrderTransactions
                .Include(a => a.Bill)
                .Include(a => a.Order)
                .FirstOrDefaultAsync(b => b.TransactionId == Ulid.Parse(model.vnp_TxnRef));

            // transaction không tồn tại
            if (transaction == null)
            {
                return Results.BadRequest("Transaction not found!");
            }

            if (transaction.Status == "Paid")
            {
                return Results.Ok("Payment is paid!");
            }

            transaction.Status = model.vnp_ResponseCode == "00" ? "Paid" : "Failed";
            transaction.Order.PaymentStatus = model.vnp_ResponseCode == "00" ? "Paid" : "Failed";
            var table = await context.Tables.FindAsync(transaction.Order.TableId);
            table.ActiveStatus = "Empty";
            bool isVoucherUsed = false;
            CustomerVoucher customerVoucher = new CustomerVoucher();
            if (!string.IsNullOrEmpty(transaction.VoucherId.ToString()))
            {
                //TODO: Cập nhật số lượng voucher cho khách hàng
                customerVoucher = await context.CustomerVouchers
                    .Include(a => a.Customer)
                    .ThenInclude(a => a.User)
                    .Where(a => a.VoucherId == transaction.VoucherId && a.Customer.User.Phone == transaction.PayerName)
                    .FirstOrDefaultAsync();
                customerVoucher.Quantity -= 1;
                isVoucherUsed = true;

            }

            if (transaction.BillId != null) //Có book bàn
            {
                //truy vấn lấy bill
                // var bill = await _context.Bills.FindAsync(transaction.BillId);
                var booking = await context.Bookings.FindAsync(transaction.Order.Bill.BookId);
                booking.BookingStatus = "Completed"; //Cập nhật trạng thái booking
                transaction.Bill.Total = decimal.Parse(model.vnp_Amount) / 100;
                transaction.Bill.PaymentStatus = model.vnp_ResponseCode == "00" ? "Paid" : "Failed";
                transaction.Bill.IsVoucherUsed = isVoucherUsed;
                transaction.Bill.OrderId = transaction.OrderId;
                transaction.Bill.PaymentType = "VNPay";
                transaction.Bill.CreatedDate = DateTime.Now;
                transaction.Bill.VoucherId = transaction.VoucherId;
            }
            else //Không book bàn
            {
                var bill = new Bill
                {
                    BillId = Ulid.NewUlid(),
                    BookId = null,
                    CreatedDate = DateTime.Now,
                    OrderId = transaction.OrderId,
                    Total = decimal.Parse(model.vnp_Amount) / 100,
                    PaymentStatus = "Paid",
                    IsVoucherUsed = isVoucherUsed,
                    VoucherId = transaction.VoucherId,
                    PaymentType = "VNPay"
                };
                await context.Bills.AddAsync(bill);
            }
            await unitOfWork.SaveChangesAsync();

            try
            {
                return Results.Redirect(configuration["RedirectURL_Orders"]!);
            }
            catch (Exception)
            {
                return Results.Ok("Payment Success!");
            }
        });

        endpoints.MapDelete("remove-transaction/{id}", async (
            string id,
            ISender sender) =>
        {
            var result = await sender.Send(new RemoveTransactionCommand(id));
            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            return Results.Ok(result);
        }).RequireAuthorization()
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

    }

}

