using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.API.Extentions;
using RestaurantManagement.Application.Features.AccountFeature.Commands.ForgotPassword;
using RestaurantManagement.Application.Features.AccountFeature.Commands.Register;
using RestaurantManagement.Application.Features.AccountFeature.Queries.ActivateAccount;
using RestaurantManagement.Application.Features.AccountFeature.Queries.Login;
using RestaurantManagement.Application.Features.AccountFeature.Queries.ResetPasswordVerify;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.API.Controllers
{

    public class AccountController : IEndpoint
    {
        
        public void MapEndpoint(IEndpointRouteBuilder app)
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

                return Results.BadRequest(result.ToProblemDetails());
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

            //login for employee

            

            //reset password
            endpoints.MapPost("reset-password", async (
                ForgotCustomerPasswordCommand command, 
                ISender sender) =>
            {
                var result = await sender.Send(command);
                if (result.IsSuccess)
                {
                    return Results.Ok("Check your email!");
                }
                return Results.BadRequest(result.Errors);
            });


            //verify email (Active account for customer)
            endpoints.MapGet("verify-email", async (Ulid token, ISender sender) =>
            {
                
                var result = await sender.Send(new ActivateAccountCommand(token));
                if (result.IsSuccess)
                {
                    return Results.Ok("Email verified successfully!");
                }

                return Results.BadRequest(result.Errors[0].Message);

            });

            //verify email to reset pass
            endpoints.MapGet("verify-reset-password", async (Ulid token, ISender sender) =>
            {
                var result = await sender.Send(new ResetPasswordVerifyCommand(token));
                if (result.IsSuccess)
                {
                    return Results.Ok("Email verified successfully!");
                }

                return Results.BadRequest(result.Errors[0].Message);

            });
        }
    }
}
