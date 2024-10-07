using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.Application.Features.CustomerFeature.Commands.UpdateCustomer;
using RestaurantManagement.Application.Features.CustomerFeature.Queries.CustomerFilter;
using RestaurantManagement.Application.Features.CustomerFeature.Queries.GetCustomerById;


namespace RestaurantManagement.API.Controllers
{
    public class CustomerController : IEndpoint
    {
    
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            var endpoints = app.MapGroup("api/customer").WithTags("Customer").DisableAntiforgery();

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
            endpoints.MapGet("{id}", async (Ulid id, ISender sender) =>
            {
                GetCustomerByIdQuery request = new GetCustomerByIdQuery(id);
                var result = await sender.Send(request);
                return Results.Ok(result);
            });

            //Update information for a customer
            endpoints.MapPut("{id}", async (
                [FromRoute] Ulid id,
                [FromForm] IFormFile? image, 
                [FromForm]string? FirstName, 
                [FromForm]string? LastName, 
                [FromForm]string? PhoneNumber, 
                [FromForm]string? Gender,
                [FromServices] ISender sender) =>
            {
                byte[] imageBytes = null!;

                if (image != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        imageBytes = memoryStream.ToArray();
                    }
                }

                var updateCustomerCommand = new UpdateCustomerCommand(
                    id,
                    FirstName,
                    LastName,
                    PhoneNumber,
                    imageBytes,
                    Gender);

                var result = await sender.Send(updateCustomerCommand);
                if(result.IsSuccess)
                {
                    return Results.Ok("Customer updated successfully!");
                }
                return Results.BadRequest(result.Errors);
            });
        }
    }

}
