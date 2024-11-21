using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.API.Authentication;
using RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetBillHistory;
using RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetBookingHistory;
using RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetCategoryHistory;
using RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetCustomerHistory;
using RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetEmployeeHistory;
using RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetMealHistory;
using RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetOrderHistory;
using RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetTableHistory;
using RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetTableTypeHistory;

namespace RestaurantManagement.API.Controllers;

public class ActivityHistoryController : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("api/history").WithTags("ActivityHistory").DisableAntiforgery();

        endpoints.MapGet("customer", async (
            [FromQuery] string? filterUserId,
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            ISender sender) =>
        {
            var result = await sender.Send(new GetCustomerHistoryQuery(
                filterUserId, 
                searchTerm, 
                sortColumn, 
                sortOrder, 
                page, 
                pageSize
            ));

            if(result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        endpoints.MapGet("employee", async (
            [FromQuery] string? filterUserId,
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeHistoryQuery(
                filterUserId, 
                searchTerm, 
                sortColumn, 
                sortOrder, 
                page, 
                pageSize
            ));

            if(result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        endpoints.MapGet("meal", async (
            [FromQuery] string? filterUserId,
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            ISender sender) =>
        {
            var result = await sender.Send(new GetMealHistoryQuery(
                filterUserId, 
                searchTerm, 
                sortColumn, 
                sortOrder, 
                page, 
                pageSize
            ));

            if(result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        endpoints.MapGet("order", async (
            [FromQuery] string? filterUserId,
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            ISender sender) =>
        {
            var result = await sender.Send(new GetOrderHistoryQuery(
                filterUserId, 
                searchTerm, 
                sortColumn, 
                sortOrder, 
                page, 
                pageSize
            ));

            if(result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        endpoints.MapGet("bill", async (
            [FromQuery] string? filterUserId,
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            ISender sender) =>
        {
            var result = await sender.Send(new GetBillHistoryQuery(
                filterUserId, 
                searchTerm, 
                sortColumn, 
                sortOrder, 
                page, 
                pageSize
            ));

            if(result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        endpoints.MapGet("booking", async (
            [FromQuery] string? filterUserId,
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            ISender sender) =>
        {
            var result = await sender.Send(new GetBookingHistoryQuery(
                filterUserId, 
                searchTerm, 
                sortColumn, 
                sortOrder, 
                page, 
                pageSize
            ));

            if(result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        endpoints.MapGet("table", async (
            [FromQuery] string? filterUserId,
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            ISender sender) =>
        {
            var result = await sender.Send(new GetTableHistoryQuery(
                filterUserId, 
                searchTerm, 
                sortColumn, 
                sortOrder, 
                page, 
                pageSize
            ));

            if(result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        endpoints.MapGet("table-type", async (
            [FromQuery] string? filterUserId,
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            ISender sender) =>
        {
            var result = await sender.Send(new GetTableTypeHistoryQuery (
                filterUserId, 
                searchTerm, 
                sortColumn, 
                sortOrder, 
                page, 
                pageSize
            ));

            if(result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        endpoints.MapGet("category", async (
            [FromQuery] string? filterUserId,
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            ISender sender) =>
        {
            var result = await sender.Send(new GetCategoryHistoryQuery(
                filterUserId, 
                searchTerm, 
                sortColumn, 
                sortOrder, 
                page, 
                pageSize
            ));

            if(result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        
    }
}
