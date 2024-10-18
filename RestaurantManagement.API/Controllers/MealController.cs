using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.API.Extentions;
using RestaurantManagement.Application.Features.MealFeature.Commands.CreateMeal;
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
            byte[] imageData = null!;
            if (image != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await image.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                }
            }

            //lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);
            var result = await sender.Send(
                new CreateMealCommand(
                    MealName, 
                    Price, 
                    imageData, 
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
            [FromQuery] string? seachTerm,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int page,
            [FromQuery] int pageSize, ISender sender) => 
        {
            var query = new GetAllMealQuery(seachTerm, sortColumn, sortOrder, page, pageSize);
            var response = await sender.Send(query);
            return Results.Ok(response);
        });
    }
}
