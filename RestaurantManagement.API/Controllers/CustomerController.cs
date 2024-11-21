using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.API.Authentication;
using RestaurantManagement.Application.Features.AccountFeature.Commands.UpdateCustomerInformation;
using RestaurantManagement.Application.Features.CustomerFeature.Commands.CreateCustomer;
using RestaurantManagement.Application.Features.CustomerFeature.Queries.CustomerFilter;
using RestaurantManagement.Application.Features.CustomerFeature.Queries.GetCustomerById;
using RestaurantManagement.Domain.IRepos;


namespace RestaurantManagement.API.Controllers;

public class CustomerController : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("api/customer").WithTags("Customer").DisableAntiforgery();

        // Get all with pagination
        endpoints.MapGet("",
        async (
            ISender sender,
            [FromQuery] string? filterUserType,
            [FromQuery] string? filterGender,
            [FromQuery] string? filterStatus,
            [FromQuery] string? searchTerm,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder) =>
        {
            CustomerFilterQuery request = new CustomerFilterQuery(filterGender, filterUserType, filterStatus, searchTerm, sortColumn, sortOrder, page, pageSize);
            var result = await sender.Send(request);
            if (result != null)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        endpoints.MapPost("", async (
            [FromForm] string FirstName,
            [FromForm] string LastName,
            [FromForm] string? Email,
            [FromForm] string? Phone,
            [FromForm] string Gender,
            ISender sender,
            HttpContext httpContext,
            IJwtProvider jwtProvider) =>
        {
            // Lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);

            // Gửi command
            var result = await sender.Send(new CreateCustomerCommand(FirstName, LastName, Email, Phone, Gender, token));
            if (result != null)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).RequireAuthorization("boss")
        .RequireRateLimiting("AntiSpamCreateCustomerCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();



        //Get customer by id
        endpoints.MapGet("{id}", async (string id, ISender sender) =>
        {
            GetCustomerByIdQuery request = new GetCustomerByIdQuery(id);
            var result = await sender.Send(request);
            return Results.Ok(result);
        })
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();


        //Delete a customer
        endpoints.MapDelete("{id}", async (
            string id,
            ISender sender) =>
        {
            // Gửi command để xóa khách hàng
            // var result = await sender.Send(new DeleteCustomerCommand(id));
            return Results.Ok();
        }).RequireRateLimiting("AntiSpamDeleteCustomerCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        //Update information for a customer
        endpoints.MapPut("{id}",
        async (
            string id,
            [FromForm] IFormFile? image,
            [FromForm] string? FirstName,
            [FromForm] string? LastName,
            [FromForm] string? PhoneNumber,
            [FromForm] string? Gender,
            ISender sender,
            HttpContext httpContext,
            IJwtProvider jwtProvider) =>
        {

            //lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);

            
            var updateCustomerCommand = new UpdateCustomerInformationCommand(
                id,
                FirstName,
                LastName,
                PhoneNumber,
                image,
                Gender,
                token
            );

            var result = await sender.Send(updateCustomerCommand);
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);

        }).RequireAuthorization("customer")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

    }
}