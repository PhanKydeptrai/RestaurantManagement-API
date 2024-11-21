using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.API.Authentication;
using RestaurantManagement.Application.Features.AccountFeature.Commands.UpdateEmployeeInformation;
using RestaurantManagement.Application.Features.EmployeeFeature.Commands.CreateEmployee;
using RestaurantManagement.Application.Features.EmployeeFeature.Commands.DeleteEmployee;
using RestaurantManagement.Application.Features.EmployeeFeature.Commands.RestoreEmloyee;
using RestaurantManagement.Application.Features.EmployeeFeature.Commands.UpdateEmployeeRole;
using RestaurantManagement.Application.Features.EmployeeFeature.Queries.GetAllEmployee;
using RestaurantManagement.Application.Features.EmployeeFeature.Queries.GetEmployeeById;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.API.Controllers;

public class EmployeeController : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("api/employee").WithTags("Employee").DisableAntiforgery();

        //Update information for employee
        endpoints.MapPut("{id}", async (
            string id,
            [FromForm] string FirstName,
            [FromForm] string LastName,
            [FromForm] string PhoneNumber,
            [FromForm] IFormFile? UserImage,
            HttpContext httpContext,
            ISender sender,
            IJwtProvider jwtProvider) =>
        {
            // Lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);
            var result = await sender.Send(
                new UpdateEmployeeInformationCommand(
                    id,
                    FirstName,
                    LastName,
                    PhoneNumber,
                    UserImage,
                    token));

            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).RequireAuthorization()
        .RequireRateLimiting("AntiSpamUpdateEmployeeInformationCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        //Create employee
        endpoints.MapPost("", async (
            [FromForm] string FirstName,
            [FromForm] string LastName,
            [FromForm] string PhoneNumber,
            [FromForm] string Email,
            [FromForm] string Role,
            [FromForm] string Gender,
            [FromForm] IFormFile? UserImage,
            ISender sender,
            HttpContext httpContext,
            IJwtProvider jwtProvider) =>
        {
            // Lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);

            var result = await sender.Send(
                new CreateEmployeeCommand(
                    FirstName,
                    LastName,
                    PhoneNumber,
                    Email,
                    UserImage,
                    Role,
                    Gender,
                    token));

            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            return Results.Ok(result);
        })
        .RequireAuthorization()
        .RequireRateLimiting("AntiSpamCreateEmployeeCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();



        //Get all
        endpoints.MapGet("",
        async (
            [FromQuery] string? filterGender,
            [FromQuery] string? filterRole,
            [FromQuery] string? filterStatus,
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            ISender sender) =>
        {
            var result = await sender.Send(new GetAllEmployeeQuery(filterGender, filterRole, filterStatus, searchTerm, sortColumn, sortOrder, page, pageSize));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        //Delete employee
        endpoints.MapDelete("{id}", async (
            string id,
            ISender sender,
            IJwtProvider jwtProvider,
            HttpContext httpContext) =>
        {
            // Lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);
            var result = await sender.Send(new DeleteEmployeeCommand(id, token));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        })
        .RequireAuthorization("management")
        .RequireRateLimiting("AntiSpamDeleteEmployeeCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        //restore employee
        endpoints.MapPut("restore-employee/{id}", async (
            string id,
            ISender sender,
            IJwtProvider jwtProvider,
            HttpContext httpContext) =>
        {
            // Lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);
            var result = await sender.Send(new RestoreEmployeeCommand(id, token));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        })
        .RequireAuthorization("management")
        .RequireRateLimiting("AntiSpamRestoreEmployeeCommand");

        //Thay đổi role cho nhân viên
        endpoints.MapPut("employee-role/{id}", async (
            string id,
            [FromForm] string role,
            ISender sender,
            IJwtProvider jwtProvider,
            HttpContext httpContext) =>
        {
            // Lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);

            var result = await sender.Send(new UpdateEmployeeRoleCommand(id, role, token));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        })
        .RequireAuthorization("management")
        .RequireRateLimiting("AntiSpamUpdateEmployeeRoleCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        //get employee by id 
        endpoints.MapGet("{id}", async (
            string id,
            ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeByIdQuery(id));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.NoContent();
        })
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

    }
}