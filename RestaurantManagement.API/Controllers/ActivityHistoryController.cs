using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetBillHistory;
using RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetBookingHistory;
using RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetCategoryHistory;
using RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetCustomerHistory;
using RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetEmployeeHistory;
using RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetMealHistory;
using RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetOrderHistory;
using RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetTableHistory;
using RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetTableTypeHistory;
using RestaurantManagement.Application.Features.MealFeature.Queries.GetMealById;
using RestaurantManagement.Application.Features.OrderFeature.Queries.GetOrderById;
using RestaurantManagement.Application.Features.TableFeature.Queries.GetTableById;

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
        });

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
        });

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
        });

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
        });

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
        });

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
        });

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
        });

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
        });

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
        });

        
    }
}
