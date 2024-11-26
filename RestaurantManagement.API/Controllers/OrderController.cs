using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.API.Authentication;
using RestaurantManagement.Application.Features.OrderFeature.Commands.AddMealToOrder;
using RestaurantManagement.Application.Features.OrderFeature.Commands.ApplyVoucher;
using RestaurantManagement.Application.Features.OrderFeature.Commands.DeleteMealFromOrder;
using RestaurantManagement.Application.Features.OrderFeature.Commands.MakePayment;
using RestaurantManagement.Application.Features.OrderFeature.Commands.PayOrder;
using RestaurantManagement.Application.Features.OrderFeature.Commands.UpdateMealInOrder;
using RestaurantManagement.Application.Features.OrderFeature.Queries.GetAllOrder;
using RestaurantManagement.Application.Features.OrderFeature.Queries.GetOrderById;
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
        });

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

        #region Old payorder
        // //Pay order
        // endpoints.MapPut("pay/{id}", async (
        //     string id, // Id bàn
        //     [FromBody] PayOrderRequest request,
        //     ISender sender) =>
        // {
        //     var result = await sender.Send(new PayOrderCommand(id, request.voucherName, request.phoneNumber));
        //     if (!result.IsSuccess)
        //     {
        //         return Results.BadRequest(result);
        //     }
        //     return Results.Ok(result);
        // })
        // .RequireAuthorization()
        // .RequireRateLimiting("AntiSpamPayOrderCommand")
        // .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();
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

        // //Áp dụng voucher
        // endpoints.MapPut("apply-voucher/{id}", async (
        //     string id,
        //     [FromBody] ApplyVoucherRequest request,
        //     ISender sender) =>
        // {
        //     var result = await sender.Send(new ApplyVoucherCommand(id, request.voucherCode, request.phoneNumber));
        //     if (!result.IsSuccess)
        //     {
        //         return Results.BadRequest(result);
        //     }
        //     return Results.Ok(result);
        // });
    }
}
