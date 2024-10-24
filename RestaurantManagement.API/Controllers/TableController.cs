using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.Application.Features.TableFeature.Commands.CreateTable;
using RestaurantManagement.Application.Features.TableFeature.Queries.GetAllTable;
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
            }).RequireAuthorization("boss");

            //Update table
            endpoints.MapPut("",
            async () =>
            {
                
            }).RequireAuthorization("boss");

            //Remove table
            endpoints.MapDelete("",
            async () =>
            {
                
            }).RequireAuthorization("boss");
        }
    }
}
