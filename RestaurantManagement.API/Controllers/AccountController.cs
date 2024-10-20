using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.API.Extentions;
using RestaurantManagement.Application.Features.AccountFeature.Commands.ActivateAccount;
using RestaurantManagement.Application.Features.AccountFeature.Commands.ChangeCustomerPassword;
using RestaurantManagement.Application.Features.AccountFeature.Commands.ForgotCustomerPassword;
using RestaurantManagement.Application.Features.AccountFeature.Commands.ForgotEmployeePassword;
using RestaurantManagement.Application.Features.AccountFeature.Commands.Register;
using RestaurantManagement.Application.Features.AccountFeature.Commands.ResetPasswordVerify;
using RestaurantManagement.Application.Features.AccountFeature.Commands.VerifyChangeCustomerPassword;
using RestaurantManagement.Application.Features.AccountFeature.Queries.EmployeeLogin;
using RestaurantManagement.Application.Features.AccountFeature.Queries.Login;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.API.Controllers
{

    public class AccountController : IEndpoint
    {

        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            var endpoints = app.MapGroup("api/account").WithTags("Account").DisableAntiforgery();

            //Register for customer
            endpoints.MapPost("register", async (
                [FromBody] RegisterCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }

                return Results.BadRequest(result.ToProblemDetails());
            });

            //Login for customer
            endpoints.MapPost("login", async (
                [FromBody] LoginQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result.ToProblemDetails());
            });

            //login for employee
            endpoints.MapPost("employee-login", async (
                [FromBody] EmployeeLoginQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result.ToProblemDetails());
            });


            //reset customer password 
            endpoints.MapPost("customer-password", async (
                ForgotCustomerPasswordCommand command,
                ISender sender) =>
            {
                var result = await sender.Send(command);
                if (result.IsSuccess)
                {
                    return Results.Ok("Check your email!");
                }
                return Results.BadRequest(result.ToProblemDetails());
            }).RequireRateLimiting("ResetPass");

            //reset employee password 
            endpoints.MapPost("employee-password", async (
                ForgotEmployeePasswordCommand command,
                ISender sender) =>
            {
                var result = await sender.Send(command);
                if (result.IsSuccess)
                {
                    return Results.Ok("Check your email!");
                }
                return Results.BadRequest(result.ToProblemDetails());
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

            //verify email to change pass
            endpoints.MapGet("verify-change-password", async (Ulid token, ISender sender) =>
            {
                var result = await sender.Send(new VerifyChangeCustomerPasswordCommand(token));
                if (result.IsSuccess)
                {
                    return Results.Ok("Change password successfully!");
                }

                return Results.BadRequest(result.Errors[0].Message);
            });

            //Change password 
            endpoints.MapPost("change-password", async (
                [FromBody] ChangePasswordRequest request,
                HttpContext httpContext,
                ISender sender,
                IJwtProvider jwtProvider) =>
            {
                var token = jwtProvider.GetTokenFromHeader(httpContext);

                var result = await sender.Send(new ChangePasswordCommand(request.oldPass, request.newPass, token));
                if (result.IsSuccess)
                {
                    return Results.Ok("Please check your email!");
                }
                return Results.BadRequest(result.ToProblemDetails());
            }).RequireAuthorization().RequireRateLimiting("ResetPass");
        }
    }
}
