using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.API.Extentions;
using RestaurantManagement.Application.Features.AccountFeature.Commands.UpdateEmployeeInformation;

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
            [FromForm]byte[]? UserImage,
            HttpContext httpContext,
            ISender sender) => 
        {
            //Lấy token
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
                    UserImage,
                    token));

            if(result.IsSuccess)
            {
                return Results.Ok();
            }
            return Results.BadRequest(result.ToProblemDetails);
        }).RequireAuthorization();
    }
}
