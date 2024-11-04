
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.Application.Features.OrderFeature.Commands.CreateOrder;

namespace RestaurantManagement.API.Controllers;

public class OrderController : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("api/orders");

        //Create order
        endpoints.MapPost("", async (
            [FromBody]CreateOrderRequest command,
            ISender sender) =>
        {       
            var result = await sender.Send(new CreateOrderCommand(
                command.TableId,
                Ulid.Parse(command.MealId),
                command.Quantity
            ));

            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            return Results.Ok(result);
        });

        
        //Get all orders
        //Get order by id
        //Get orders by table id
        //Update order
        //Delete order

    }
}
