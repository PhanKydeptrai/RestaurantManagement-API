using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.API.Authentication;
using RestaurantManagement.Application.Features.TableFeature.Commands.AssignTableToBookedCustomer;
using RestaurantManagement.Application.Features.TableFeature.Commands.AssignTableToCustomer;
using RestaurantManagement.Application.Features.TableFeature.Commands.ChangeTable;
using RestaurantManagement.Application.Features.TableFeature.Commands.CreateTable;
using RestaurantManagement.Application.Features.TableFeature.Commands.DeleteTable;
using RestaurantManagement.Application.Features.TableFeature.Commands.RestoreTable;
using RestaurantManagement.Application.Features.TableFeature.Commands.UnAssignTableToBookedCustomerCommand;
using RestaurantManagement.Application.Features.TableFeature.Commands.UnAssignTableToCustomer;
using RestaurantManagement.Application.Features.TableFeature.Queries.GetAllTable;
using RestaurantManagement.Application.Features.TableFeature.Queries.GetTableById;
using RestaurantManagement.Application.Features.TableFeature.Queries.GetTableInfo;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.API.Controllers;

public class TableController : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("api/table").WithTags("Table").DisableAntiforgery();

        //Get all tables
        endpoints.MapGet("",
        async (
            [FromQuery] string? filterTableType,
            [FromQuery] string? filterActiveStatus,
            [FromQuery] string? filterStatus,
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            ISender sender) =>
        {

            var result = await sender.Send(
                new GetAllTableQuery(
                    filterTableType,
                    filterActiveStatus,
                    filterStatus,
                    searchTerm,
                    sortColumn,
                    sortOrder,
                    page,
                    pageSize));

            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            return Results.Ok(result);

        }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        endpoints.MapGet("{id}", async (
            string id,
            ISender sender) =>
        {
            var result = await sender.Send(new GetTableByIdQuery(id));
            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            return Results.Ok(result);
        }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        //Create table
        endpoints.MapPost("", async (
            [FromBody] CreateTableRequest request,
            IJwtProvider jwtProvider,
            HttpContext httpContext,
            ISender sender) =>
        {
            // Lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);
            var result = await sender.Send(new CreateTableCommand(
                request.quantity,
                request.tableTypeId,
                token));

            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            return Results.Ok(result);
        })
        .RequireAuthorization("boss")
        .RequireRateLimiting("AntiSpamCreateTableCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();


        //Remove table
        endpoints.MapDelete("{id}", async (
            string id,
            ISender sender,
            HttpContext httpContext,
            IJwtProvider jwtProvider) =>
        {
            // Lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);
            var result = await sender.Send(new DeleteTableCommand(id, token));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        })
        .RequireAuthorization("boss")
        .RequireRateLimiting("AntiSpamDeleteTableCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        //Restore table
        endpoints.MapPut("{id}", async (
            string id,
            ISender sender,
            HttpContext httpContext,
            IJwtProvider jwtProvider) =>
        {
            // Lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);
            var result = await sender.Send(new RestoreTableCommand(id, token));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        })
        .RequireAuthorization("boss")
        .RequireRateLimiting("AntiSpamRestoreTableCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();


        // Cho khách vãng lai nhận bàn
        endpoints.MapPut("table-assign/{id}", async (
            string id,
            ISender sender,
            HttpContext httpContext,
            IJwtProvider jwtProvider) =>
        {
            // Lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);
            var result = await sender.Send(new AssignTableToCustomerCommand(id, token));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        })
        .RequireAuthorization()
        .RequireRateLimiting("AntiSpamAssignTableToCustomerCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        //  thôi nhận bàn cho khách đã không book bàn (trong trường hợp nhân viên bấm nhầm)
        endpoints.MapPut("table-unassign/{id}", async (
            string id,
            ISender sender) =>
        {
            var result = await sender.Send(new UnAssignTableToCustomerCommand(id));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        })
        .RequireAuthorization()
        .RequireRateLimiting("AntiSpamUnAssignTableToCustomerCommand");


        endpoints.MapPut("table-assign/booked/{id}", async (
            string id,
            ISender sender,
            HttpContext httpContext,
            IJwtProvider jwtProvider) =>
        {
            // Lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);
            var result = await sender.Send(new AssignTableToBookedCustomerCommand(id, token));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        })
        .RequireAuthorization()
        .RequireRateLimiting("AntiSpamAssignTableToBookedCustomerCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        // thôi nhận bàn cho khách đã book bàn (trong trường hợp nhân viên bấm nhầm)
        endpoints.MapPut("table-unassign/booked/{id}", async (
            string id,
            ISender sender) =>
        {
            var result = await sender.Send(new UnAssignTableToBookedCustomerCommand(id));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        })
        .RequireAuthorization()
        .RequireRateLimiting("AntiSpamUnAssignTableToBookedCustomerCommand");

        endpoints.MapGet("table-info", async (ISender sender) =>
        {
            var result = await sender.Send(new GetTableInfoQuery());
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).RequireAuthorization()
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();


        endpoints.MapPut("change-table/{id}", async (
            string id,
            [FromBody] ChangeTableRequest request,
            ISender sender,
            HttpContext httpContext,
            IJwtProvider jwtProvider) =>
        {
            
            // Lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);
            var result = await sender.Send(new ChangeTableCommand(
                id,
                request.note,
                request.newTableId,
                token));

            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            return Results.Ok(result);
        });

    }
}
