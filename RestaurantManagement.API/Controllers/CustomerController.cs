using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.Application.Features.AccountFeature.Commands.UpdateCustomerInformation;
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
                Ulid id,
                [FromForm] IFormFile? image, 
                [FromForm]string? FirstName, 
                [FromForm]string? LastName, 
                [FromForm]string? PhoneNumber, 
                [FromForm]string? Gender,
                ISender sender, 
                HttpContext httpContext) =>
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

                //lấy token
                var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
                //Trích xuất token
                var token = authHeader.Substring("Bearer ".Length).Trim();

                var updateCustomerCommand = new UpdateCustomerInformationCommand(
                    id,
                    FirstName,
                    LastName,
                    PhoneNumber,
                    imageBytes,
                    Gender,
                    token);

                var result = await sender.Send(updateCustomerCommand);
                if(result.IsSuccess)
                {
                    return Results.Ok("Customer updated successfully!");
                }
                return Results.BadRequest(result.Errors);

            }).RequireAuthorization();
        }
    }

}
