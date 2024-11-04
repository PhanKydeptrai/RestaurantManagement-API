using RestaurantManagement.API.Abstractions;

namespace RestaurantManagement.API.Controllers;

public class OrderController : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("api/orders");

        //Create order
        
        //Get all orders
        //Get order by id
        //Get orders by table id
        //Update order
        //Delete order

    }
}
