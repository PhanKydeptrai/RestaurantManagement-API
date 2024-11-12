using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.Application.Features.MealFeature.Commands.ChangeSellStatus;
using RestaurantManagement.Application.Features.MealFeature.Commands.CreateMeal;
using RestaurantManagement.Application.Features.MealFeature.Commands.RemoveMeal;
using RestaurantManagement.Application.Features.MealFeature.Commands.RestoreMeal;
using RestaurantManagement.Application.Features.MealFeature.Commands.RestoreSellStatus;
using RestaurantManagement.Application.Features.MealFeature.Commands.UpdateMeal;
using RestaurantManagement.Application.Features.MealFeature.Queries.GetAllMeal;
using RestaurantManagement.Application.Features.MealFeature.Queries.GetMealById;
using RestaurantManagement.Application.Features.MealFeature.Queries.GetMealInfo;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.API.Controllers;

public class MealController : IEndpoint
{
    //Add role
    public async void MapEndpoint(IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("api/meal").WithTags("Meal").DisableAntiforgery().RequireRateLimiting("AntiSpam");

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
                    CategoryId,
                    token));

            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            return Results.Ok(result);

        }).RequireAuthorization("boss");

        //Get all meal
        endpoints.MapGet("",
        async (
            [FromQuery] string? filterCategory,
            [FromQuery] string? filterSellStatus,
            [FromQuery] string? filterMealStatus,
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int? page,
            [FromQuery] int? pageSize, ISender sender) =>
        {
            var query = new GetAllMealQuery(filterCategory, filterSellStatus, filterMealStatus, searchTerm, sortColumn, sortOrder, page, pageSize);
            var response = await sender.Send(query);
            return Results.Ok(response);
        });

        //get meal by id
        endpoints.MapGet("{id}",
        async (
            string id,
            ISender sender) =>
        {
            var result = await sender.Send(new GetMealByIdQuery(id));
            if (result.IsSuccess && result.Value != null)
            {
                return Results.Ok(result);
            }
            return Results.NoContent();
        });

        //Update meal
        endpoints.MapPut("{id}",
        async (
            string id,
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
                id, MealName, Price, image, Description, CategoryId, token));

            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).RequireAuthorization("boss");


        //Xóa món
        endpoints.MapDelete("{id}",
        async (
            string id,
            ISender sender,
            HttpContext httpContext,
            IJwtProvider jwtProvider) =>
        {
            //lấy token
            string token = jwtProvider.GetTokenFromHeader(httpContext);

            //gửi lệnh xóa
            var result = await sender.Send(new RemoveMealCommand(id, token));

            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);

        }).RequireAuthorization("boss");

        //khôi phục món
        endpoints.MapPut("restore/{id}",
        async (
            string id,
            ISender sender,
            HttpContext httpContext,
            IJwtProvider jwtProvider) =>
        {
            //lấy token
            string token = jwtProvider.GetTokenFromHeader(httpContext);

            var result = await sender.Send(new RestoreMealCommand(id, token));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).RequireAuthorization("boss")
        ;

        //Chuyển sell status Active => InActive
        endpoints.MapPut("change-sellstatus/{id}",
        async (
            string id,
            ISender sender,
            HttpContext httpContext,
            IJwtProvider jwtProvider) =>
        {
            //lấy token
            string token = jwtProvider.GetTokenFromHeader(httpContext);

            var result = await sender.Send(new ChangeSellStatusCommand(id, token));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).RequireAuthorization("boss");

        //Chuyển sell status InActive => Active
        endpoints.MapPut("restore-sellstatus/{id}",
        async (
            string id,
            ISender sender,
            HttpContext httpContext,
            IJwtProvider jwtProvider) =>
        {
            //lấy token
            string token = jwtProvider.GetTokenFromHeader(httpContext);

            var result = await sender.Send(new RestoreSellStatusCommand(id, token));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).RequireAuthorization("boss");

        endpoints.MapGet("meal-info", async (
            [FromQuery] string? searchTerm,
            ISender sender) =>
        {

            var result = await sender.Send(new GetMealInfoQuery(searchTerm));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        });
    }
}
