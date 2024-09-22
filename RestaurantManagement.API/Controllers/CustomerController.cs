using MediatR;
using RestaurantManagement.Application.Features.CustomerFeature.Commands.CreateCustomer;
using RestaurantManagement.Application.Features.CustomerFeature.Queries.CustomerFilter;
using RestaurantManagement.Application.Features.CustomerFeature.Queries.GetCustomerById;

namespace RestaurantManagement.API.Controllers
{
    public static class CustomerController
    {
        public static void MapCustomerEndpoint(this IEndpointRouteBuilder app)
        {
            var endpoints = app.MapGroup("api/customer").WithTags("Customer");

            // Get all with pagination
            endpoints.MapGet("", async (ISender sender, string? seachTerm, int page, int pageSize) =>
            {
                CustomerFilterQuery request = new CustomerFilterQuery(seachTerm, page, pageSize);
                var result = await sender.Send(request);
                if (result != null)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest("Not Found");
            });

            //Get by id
            endpoints.MapGet("{id}", async (Guid id, ISender sender) =>
            {
                GetCustomerByIdQuery request = new GetCustomerByIdQuery(id);
                var result = await sender.Send(request);
                return Results.Ok(result);
            });
            //Create new
            endpoints.MapPost("",async (CreateCustomerCommand request, ISender sender) =>
            {
                var result = await sender.Send(request);
                if (result.IsSuccess)
                {
                    return Results.Ok("Customer created successfully!");
                }
                return Results.BadRequest(result.Errors);
            });
            //Update infomation
        }

        
        

    }

}
