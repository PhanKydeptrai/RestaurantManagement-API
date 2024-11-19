using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetCustomerHistory;

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
            // var result = await sender.Send(new GetCustomerHistoryQuery(
            //     filterUserId, 
            //     searchTerm, 
            //     sortColumn, 
            //     sortOrder, 
            //     page, 
            //     pageSize
            // ));

            // if(result.IsSuccess)
            // {
            //     return Results.Ok(result);
            // }
            // return Results.BadRequest(result);
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
            // var result = await sender.Send(new GetCustomerHistoryQuery(
            //     filterUserId, 
            //     searchTerm, 
            //     sortColumn, 
            //     sortOrder, 
            //     page, 
            //     pageSize
            // ));

            // if(result.IsSuccess)
            // {
            //     return Results.Ok(result);
            // }
            // return Results.BadRequest(result);
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
            // var result = await sender.Send(new GetCustomerHistoryQuery(
            //     filterUserId, 
            //     searchTerm, 
            //     sortColumn, 
            //     sortOrder, 
            //     page, 
            //     pageSize
            // ));

            // if(result.IsSuccess)
            // {
            //     return Results.Ok(result);
            // }
            // return Results.BadRequest(result);
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
            // var result = await sender.Send(new GetCustomerHistoryQuery(
            //     filterUserId, 
            //     searchTerm, 
            //     sortColumn, 
            //     sortOrder, 
            //     page, 
            //     pageSize
            // ));

            // if(result.IsSuccess)
            // {
            //     return Results.Ok(result);
            // }
            // return Results.BadRequest(result);
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
            // var result = await sender.Send(new GetCustomerHistoryQuery(
            //     filterUserId, 
            //     searchTerm, 
            //     sortColumn, 
            //     sortOrder, 
            //     page, 
            //     pageSize
            // ));

            // if(result.IsSuccess)
            // {
            //     return Results.Ok(result);
            // }
            // return Results.BadRequest(result);
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
            // var result = await sender.Send(new GetCustomerHistoryQuery(
            //     filterUserId, 
            //     searchTerm, 
            //     sortColumn, 
            //     sortOrder, 
            //     page, 
            //     pageSize
            // ));

            // if(result.IsSuccess)
            // {
            //     return Results.Ok(result);
            // }
            // return Results.BadRequest(result);
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
            // var result = await sender.Send(new GetCustomerHistoryQuery(
            //     filterUserId, 
            //     searchTerm, 
            //     sortColumn, 
            //     sortOrder, 
            //     page, 
            //     pageSize
            // ));

            // if(result.IsSuccess)
            // {
            //     return Results.Ok(result);
            // }
            // return Results.BadRequest(result);
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
            // var result = await sender.Send(new GetCustomerHistoryQuery(
            //     filterUserId, 
            //     searchTerm, 
            //     sortColumn, 
            //     sortOrder, 
            //     page, 
            //     pageSize
            // ));

            // if(result.IsSuccess)
            // {
            //     return Results.Ok(result);
            // }
            // return Results.BadRequest(result);
        });

        
    }
}
