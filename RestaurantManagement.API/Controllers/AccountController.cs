using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.Application.Features.AccountFeature.Commands.ActivateAccount;
using RestaurantManagement.Application.Features.AccountFeature.Commands.ChangeCustomerPassword;
using RestaurantManagement.Application.Features.AccountFeature.Commands.ConfirmDeleteCustomerAccount;
using RestaurantManagement.Application.Features.AccountFeature.Commands.DeleteCustomerAccount;
using RestaurantManagement.Application.Features.AccountFeature.Commands.ForgotCustomerPassword;
using RestaurantManagement.Application.Features.AccountFeature.Commands.ForgotEmployeePassword;
using RestaurantManagement.Application.Features.AccountFeature.Commands.Register;
using RestaurantManagement.Application.Features.AccountFeature.Commands.ResetPasswordVerify;
using RestaurantManagement.Application.Features.AccountFeature.Commands.VerifyChangeCustomerPassword;
using RestaurantManagement.Application.Features.AccountFeature.Queries.DecodeToken;
using RestaurantManagement.Application.Features.AccountFeature.Queries.EmployeeLogin;
using RestaurantManagement.Application.Features.AccountFeature.Queries.GetEmployeeAccountInfo;
using RestaurantManagement.Application.Features.AccountFeature.Queries.Login;
using RestaurantManagement.Application.Features.CustomerFeature.Queries.GetCustomerById;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.API.Controllers
{

    public class AccountController : IEndpoint
    {

        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            var endpoints = app.MapGroup("api/account").WithTags("Account").DisableAntiforgery().RequireRateLimiting("AntiSpam");

            //Register for customer
            endpoints.MapPost("register",
            async (
                [FromBody] RegisterCommand command,
                ISender sender) =>
            {
                var result = await sender.Send(command);
                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }

                return Results.BadRequest(result);
            });

            //Login for customer
            endpoints.MapPost("login",
            async (
                [FromBody] LoginQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            });

            //login for employee
            endpoints.MapPost("employee-login",
            async (
                [FromBody] EmployeeLoginQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            });


            //reset customer password 
            endpoints.MapPost("customer-password",
            async (
                ForgotCustomerPasswordCommand command,
                ISender sender) =>
            {
                var result = await sender.Send(command);
                if (result.IsSuccess)
                {
                    return Results.Ok("Check your email!");
                }
                return Results.BadRequest(result);
            }).RequireRateLimiting("ResetPass");

            //reset employee password 
            endpoints.MapPost("employee-password",
            async (
                ForgotEmployeePasswordCommand command,
                ISender sender) =>
            {
                var result = await sender.Send(command);
                if (result.IsSuccess)
                {
                    return Results.Ok("Check your email!");
                }
                return Results.BadRequest(result);
            }).RequireRateLimiting("ResetPass");

            //verify email (Active account for customer)
            endpoints.MapGet("verify-email", async (Ulid token, ISender sender) =>
            {

                var result = await sender.Send(new ActivateAccountCommand(token));
                if (result.IsSuccess)
                {
                    return Results.Ok("Email verified successfully!");
                }

                return Results.BadRequest(result.Errors[0].Message);

            }).WithName("verify-email");

            //verify email to reset pass
            endpoints.MapGet("verify-reset-password", async (Ulid token, ISender sender) =>
            {
                var result = await sender.Send(new ResetPasswordVerifyCommand(token));
                if (result.IsSuccess)
                {
                    return Results.Ok("Email verified successfully!");
                }

                return Results.BadRequest(result.Errors[0].Message);

            }).WithName("verify-reset-password");

            //verify email to change pass
            endpoints.MapGet("verify-change-password", async (Ulid token, ISender sender) =>
            {
                var result = await sender.Send(new VerifyChangeCustomerPasswordCommand(token));
                if (result.IsSuccess)
                {
                    return Results.Ok("Change password successfully!");
                }

                return Results.BadRequest(result.Errors[0].Message);
            }).WithName("verify-change-password");

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
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            }).RequireAuthorization().RequireRateLimiting("ResetPass");

            //Delete customer account
            endpoints.MapDelete("customer", async (
                HttpContext httpContext,
                ISender sender,
                IJwtProvider jwtProvider) =>
            {
                //lấy token
                var token = jwtProvider.GetTokenFromHeader(httpContext);
                var result = await sender.Send(new DeleteCustomerAccountCommand(token));
                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result.Errors[0].Message);

            }).RequireAuthorization("customer");

            //verify delete customer account
            endpoints.MapGet("customer/confirm-delete-account", async (
                Ulid token,
                ISender sender) =>
            {
                var result = await sender.Send(new ConfirmDeleteCustomerAccountCommand(token));
                if (result.IsSuccess)
                {
                    return Results.Ok("Account deleted successfully!");
                }

                return Results.BadRequest(result);
            }).WithName("customer/confirm-delete-account");

            //get employee account info
            endpoints.MapGet("account-emp-info", async (
                ISender sender,
                HttpContext httpContext,
                IJwtProvider jwtProvider) =>
            {
                //lấy token
                var token = jwtProvider.GetTokenFromHeader(httpContext);
                var result = await sender.Send(new GetEmployeeAccountInfoQuery(token));
                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            }).RequireAuthorization();


            //get customer account info
            endpoints.MapGet("account-cus-info", async (
                ISender sender,
                HttpContext httpContext,
                IJwtProvider jwtProvider) =>
            {
                //lấy token
                var token = jwtProvider.GetTokenFromHeader(httpContext);
                var userId = await sender.Send(new DecodeTokenQuery(token));
                var result = await sender.Send(new GetCustomerByIdQuery(userId.Value));
                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            }).RequireAuthorization();

            //Decode jwt token
            endpoints.MapGet("decode", async (
                string token,
                ISender sender) =>
            {
                var result = await sender.Send(new DecodeTokenQuery(token));
                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            });

            #region Google authen
            endpoints.MapPost("test/{token}", async (
                string token,
                ISender sender) =>
            {
                var googleUser = await GoogleJsonWebSignature.ValidateAsync(token, new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { "512717206233-ul0hu3t49c9o30rf4lqueiqrliddrk7q.apps.googleusercontent.com" }
                });
                return Results.Ok();
            });
            #endregion

        }
    }
}
