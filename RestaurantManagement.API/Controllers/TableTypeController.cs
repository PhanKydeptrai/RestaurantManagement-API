using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.Application.Features.TableTypeFeature.Commands.CreateTableType;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.API.Controllers;

public class TableTypeController : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        var endpoints = builder.MapGroup("api/tabletype").WithTags("TableType").DisableAntiforgery();

        //Create table type
        endpoints.MapPost("", 
        async (
            [FromForm] string TableTypeName,
            [FromForm] decimal TablePrice,
            [FromForm] string? Description,
            [FromForm] IFormFile? Image,
            HttpContext httpContext,
            ISender sender,
            IJwtProvider jwtProvider) =>
        {
            //lấy token
            string token = jwtProvider.GetTokenFromHeader(httpContext);

            var result = await sender.Send(new CreateTableTypeCommand(TableTypeName, Image, TablePrice, Description, token));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);

        }).RequireAuthorization("boss");
    }
}
