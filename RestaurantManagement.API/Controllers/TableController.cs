using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.Application.Features.TableFeature.Commands.CreateTable;
using RestaurantManagement.Application.Features.TableFeature.Commands.DeleteTable;
using RestaurantManagement.Application.Features.TableFeature.Commands.RestoreTable;
using RestaurantManagement.Application.Features.TableFeature.Queries.GetAllTable;
using RestaurantManagement.Application.Features.TableFeature.Queries.GetTableById;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.API.Controllers
{
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
                //[FromQuery] string? searchTerm,
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
                        //searchTerm,
                        sortColumn,
                        sortOrder,
                        page,
                        pageSize));

                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result);
                }
                return Results.Ok(result);

            });
            
            endpoints.MapGet("{id}", async (
                string id,
                ISender sender) =>
            {
                var result = await sender.Send(new GetTableByIdQuery(int.Parse(id)));
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result);
                }
                return Results.Ok(result);
            });

            //Create table
            endpoints.MapPost("",
            async (
                [FromBody] CreateTableRequest request,
                IJwtProvider jwtProvider,
                HttpContext httpContext,
                ISender sender) =>
            {
                //lấy token
                var token = jwtProvider.GetTokenFromHeader(httpContext);
                var result = await sender.Send(new CreateTableCommand(
                    request.quantity,
                    Ulid.Parse(request.tableTypeId),
                    token));

                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result);
                }
                return Results.Ok(result);
            }).RequireAuthorization("boss")
            .RequireRateLimiting("AntiSpam");


            //Remove table
            endpoints.MapDelete("{id}",
            async (
                string id,
                ISender sender,
                HttpContext httpContext,
                IJwtProvider jwtProvider) =>
            {
                //lấy token
                var token = jwtProvider.GetTokenFromHeader(httpContext);
                var result = await sender.Send(new DeleteTableCommand(int.Parse(id), token));
                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);

            }).RequireAuthorization("boss")
            .RequireRateLimiting("AntiSpam");

            //Restore table
            endpoints.MapPut("{id}",
            async (
                string id,
                ISender sender,
                HttpContext httpContext,
                IJwtProvider jwtProvider) =>
            {
                //lấy token
                var token = jwtProvider.GetTokenFromHeader(httpContext);
                var result = await sender.Send(new RestoreTableCommand(int.Parse(id), token));
                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);

            }).RequireAuthorization("boss")
            .RequireRateLimiting("AntiSpam");
        }
    }
}
