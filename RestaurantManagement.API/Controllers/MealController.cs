using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.API.Extentions;
using RestaurantManagement.Application.Features.MealFeature.Commands.CreateMeal;
using RestaurantManagement.Application.Features.MealFeature.Commands.UpdateMeal;
using RestaurantManagement.Application.Features.MealFeature.Queries.GetAllMeal;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.API.Controllers;

public class MealController : IEndpoint
{
    //Add role
    public async void MapEndpoint(IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("api/meal").WithTags("Meal").DisableAntiforgery();

        endpoints.MapPost("",
        async (
            [FromForm] string MealName,
            [FromForm] decimal Price,
            [FromForm] IFormFile? image,
            [FromForm] string? Description,
            [FromForm] string CategoryId,
            HttpContext httpContext,
            ISender sender,
            IJwtProvider jwtProvider) =>
        {
            
            //lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);
            var result = await sender.Send(
                new CreateMealCommand(
                    MealName,
                    Price,
                    image,
                    Description,
                    Ulid.Parse(CategoryId),
                    token));

            if (!result.IsSuccess)
            {
                return Results.BadRequest(result.ToProblemDetails());
            }
            return Results.Ok(result);

        }).RequireAuthorization("boss");

        //Get all meal
        endpoints.MapGet("",
        async (
            [FromQuery] string? filterSellStatus,
            [FromQuery] string? filterMealStatus,
            [FromQuery] string? seachTerm,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int? page,
            [FromQuery] int? pageSize, ISender sender) =>
        {
            var query = new GetAllMealQuery(filterSellStatus, filterMealStatus, seachTerm, sortColumn, sortOrder, page, pageSize);
            var response = await sender.Send(query);
            return Results.Ok(response);
        });

        //Update meal
        endpoints.MapPut("{id}",
        async (
            Ulid id,
            [FromForm] string MealName,
            [FromForm] decimal Price,
            [FromForm] IFormFile? image,
            [FromForm] string? Description,
            [FromForm] string CategoryId,
            HttpContext httpContext,
            IJwtProvider jwtProvider,
            ISender sender) =>
        {
            //lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);

            var result = await sender.Send(new UpdateMealCommand(
                id, MealName, Price, image, Description, Ulid.Parse(CategoryId), token));

            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result.ToProblemDetails);
        }).RequireAuthorization("boss");
    }
}
