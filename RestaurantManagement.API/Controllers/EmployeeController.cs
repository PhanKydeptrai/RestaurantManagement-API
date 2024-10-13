using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.API.Extentions;
using RestaurantManagement.Application.Features.AccountFeature.Commands.UpdateEmployeeInformation;
using RestaurantManagement.Application.Features.EmployeeFeature.Commands.CreateEmployee;

namespace RestaurantManagement.API.Controllers;

public class EmployeeController : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("api/employee").WithTags("Employee").DisableAntiforgery();

        //Update information for employee
        endpoints.MapPut("{id}", async (
            Ulid id, 
            [FromForm]string FirstName,
            [FromForm]string LastName,
            [FromForm]string PhoneNumber,
            [FromForm]IFormFile? UserImage,
            HttpContext httpContext,
            ISender sender) => 
        {
            byte[] imageData = null!;
            if (UserImage != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await UserImage.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                }
            }
            //lấy token
            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
            //Trích xuất token
            var token = authHeader.Substring("Bearer ".Length).Trim();

            var result = await sender.Send(
                new UpdateEmployeeInformationCommand(
                    id, 
                    FirstName, 
                    LastName, 
                    PhoneNumber, 
                    imageData,
                    token));

            if(result.IsSuccess)
            {
                return Results.Ok();
            }
            return Results.BadRequest(result.ToProblemDetails);
        }).RequireAuthorization();

        //Create employee
        endpoints.MapPost("", async(
            [FromForm] string FirstName,
            [FromForm] string LastName,
            [FromForm] string Password,
            [FromForm] string PhoneNumber,
            [FromForm] string Email,
            [FromForm] string Role,
            [FromForm] string Gender,
            [FromForm] IFormFile? UserImage,
            ISender sender) => 
        {
            byte[] imageData = null!;
            if (UserImage != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await UserImage.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                }
            }

            var result = await sender.Send(
                new CreateEmployeeCommand(
                    FirstName, 
                    LastName,
                    PhoneNumber, 
                    Email, 
                    imageData, 
                    Role,
                    Gender));

            if (!result.IsSuccess)
            {
                return Results.BadRequest(result.Errors);
            }
            return Results.Ok("Create employee successfully");

        });
    
    }
}
