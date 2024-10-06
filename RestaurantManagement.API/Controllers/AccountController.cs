using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Extentions;
using RestaurantManagement.Application.Features.AccountFeature.Commands.Register;
using RestaurantManagement.Application.Features.AccountFeature.Queries.Login;

namespace RestaurantManagement.API.Controllers
{
    
    public static class AccountController
    {
        public static void MapAccountEnpoint(this IEndpointRouteBuilder app)
        {
            var endpoints = app.MapGroup("api/account").WithTags("Account").DisableAntiforgery();
            //Register for customer
            endpoints.MapPost("register", async(
                [FromBody]RegisterCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                if (result.IsSuccess)
                {
                    return Results.Ok("Register successfully!");
                }
                return Results.BadRequest(result.ToProblemDetails);
            });

            //Login for customer
            endpoints.MapPost("login", async([FromBody]LoginQuery query, ISender sender) =>
            {
                var result = await sender.Send(query);
                if (result.IsSuccess)
                {
                    return Results.Ok(result.Value);
                }
                return Results.BadRequest(result.Errors);
            });


        }
    }
}
