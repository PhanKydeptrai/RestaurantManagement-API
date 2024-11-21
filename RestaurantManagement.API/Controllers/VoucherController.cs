using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.API.Authentication;
using RestaurantManagement.Application.Features.VoucherFeature.Commands.CreateVoucher;
using RestaurantManagement.Application.Features.VoucherFeature.Commands.DeleteVoucher;
using RestaurantManagement.Application.Features.VoucherFeature.Commands.UpdateVoucher;
using RestaurantManagement.Application.Features.VoucherFeature.Queries.GetAllVoucher;
using RestaurantManagement.Application.Features.VoucherFeature.Queries.GetVoucherById;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.API.Controllers;

public class VoucherController : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        var endpoints = builder.MapGroup("api/voucher").WithTags("Voucher").DisableAntiforgery();

        endpoints.MapGet("", async (
            [FromQuery] string? filterStatus,
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,

            ISender sender) =>
        {
            //lấy token
            var result = await sender.Send(new GetAllVoucherQuery(
                filterStatus,
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

        }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        endpoints.MapPost("", async (
            [FromBody] CreateVoucherRequest request,
            ISender sender,
            HttpContext httpContext,
            IJwtProvider jwtProvider) =>
        {
            //lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);

            var result = await sender.Send(new CreateVoucherCommand(
                request.VoucherName,
                request.MaxDiscount,
                request.VoucherCondition,
                request.StartDate,
                request.ExpiredDate,
                request.Description,
                token));

            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);

        })
        .RequireAuthorization("boss")
        .RequireRateLimiting("AntiSpamCreateVoucherCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        endpoints.MapPut("{id}", async (
            string id,
            [FromBody] UpdateVoucherRequest request,
            ISender sender,
            IJwtProvider jwtProvider,
            HttpContext httpContext) =>
        {
            //lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);

            var result = await sender.Send(new UpdateVoucherCommand(
                id,
                request.VoucherName,
                request.MaxDiscount,
                request.VoucherCondition,
                request.StartDate,
                request.ExpiredDate,
                request.Description,
                token));

            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);

        })
        .RequireAuthorization("boss")
        .RequireRateLimiting("AntiSpamUpdateVoucherCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        endpoints.MapDelete("{id}", async (
            string id,
            ISender sender,
            IJwtProvider jwtProvider,
            HttpContext httpContext) =>
        {
            //lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);

            var result = await sender.Send(new DeleteVoucherCommand(id, token));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);

        })
        .RequireAuthorization("boss")
        .RequireRateLimiting("AntiSpamDeleteVoucherCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        endpoints.MapGet("{id}", async (
            string id,
            ISender sender) =>
        {
            var result = await sender.Send(new GetVoucherByIdQuery(id));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.NoContent();
        })
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();
    }



}
