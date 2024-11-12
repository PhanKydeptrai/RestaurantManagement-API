using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.Application.Features.OrderFeature.Commands.AddMealToOrder;
using RestaurantManagement.Application.Features.OrderFeature.Commands.DeleteMealFromOrder;
using RestaurantManagement.Application.Features.OrderFeature.Commands.PayOrder;
using RestaurantManagement.Application.Features.OrderFeature.Commands.UpdateMealInOrder;
using RestaurantManagement.Application.Features.OrderFeature.Queries.GetAllOrder;
using RestaurantManagement.Application.Features.OrderFeature.Queries.GetOrderById;

namespace RestaurantManagement.API.Controllers;

public class OrderController : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("api/orders").WithTags("Orders").DisableAntiforgery().RequireRateLimiting("AntiSpam");

        //Create order
        endpoints.MapPost("{id}", async (
            string id,
            [FromBody] AddMealToOrderRequest command,
            ISender sender) =>
        {
            var result = await sender.Send(new AddMealToOrderCommand(
                id,
                command.MealId,
                command.Quantity
            ));

            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            return Results.Ok(result);
        }).RequireAuthorization();

        endpoints.MapPut("{id}", async (
            string id,
            [FromBody] UpdateMealInOrderRequest command,
            ISender sender) =>
        {
            var result = await sender.Send(new UpdateMealInOrderCommand(id, command.Quantity));
            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            return Results.Ok(result);
        }).RequireAuthorization();


        endpoints.MapDelete("{id}", async (
            string id,
            ISender sender) =>
        {
            var result = await sender.Send(new DeleleMealFromOrderCommand(id));
            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            return Results.Ok(result);
        }).RequireAuthorization();


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
        });


        //Pay order
        endpoints.MapPut("pay/{id}", async (
            string id, //Id bàn
            ISender sender) =>
        {
            var result = await sender.Send(new PayOrderCommand(id));
            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            return Results.Ok(result);
        }).RequireAuthorization();

        //Get all order
        endpoints.MapGet("", async (
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
        });
    }
}
