using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.API.Authentication;
using RestaurantManagement.Application.Features.CategoryFeature.Commands.CreateCategory;
using RestaurantManagement.Application.Features.CategoryFeature.Commands.RemoveCategory;
using RestaurantManagement.Application.Features.CategoryFeature.Commands.RemoveManyCategory;
using RestaurantManagement.Application.Features.CategoryFeature.Commands.RestoreCategory;
using RestaurantManagement.Application.Features.CategoryFeature.Commands.RestoreManyCategory;
using RestaurantManagement.Application.Features.CategoryFeature.Commands.UpdateCategory;
using RestaurantManagement.Application.Features.CategoryFeature.Queries.CategoryFilter;
using RestaurantManagement.Application.Features.CategoryFeature.Queries.GetAllCategoryInfo;
using RestaurantManagement.Application.Features.CategoryFeature.Queries.GetCategoryById;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.API.Controllers;
public class CategoryController : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // Endpoint with api key authentication
        var endpoints = app.MapGroup("api/category")
            .WithTags("Category")
            .DisableAntiforgery();



        //Lấy danh sách category
        endpoints.MapGet("",
        async (
            [FromQuery] string? filter,
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int? page,
            [FromQuery] int? pageSize, ISender sender) =>
        {
            var query = new GetAllCategoryQuery(filter, searchTerm, sortColumn, sortOrder, page, pageSize);
            var response = await sender.Send(query);
            return Results.Ok(response);

        }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        //Lấy category theo id
        endpoints.MapGet("{id}", async (Ulid id, ISender sender) =>
        {
            var response = await sender.Send(new GetCategoryByIdQuery(id));
            if (response.IsSuccess)
            {
                return Results.Ok(response);
            }
            return Results.BadRequest(response);
        }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();;


        //Tạo mới category
        endpoints.MapPost("",
        async (
            [FromForm] IFormFile? Image,
            [FromForm] string name,
            [FromForm] string? description,
            HttpContext httpContext,
            ISender sender,
            IUnitOfWork unitOfWork,
            IJwtProvider jwtProvider) =>
        {
            //lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);

            //Gửi command
            var command = new CreateCategoryCommand(name, Image, token);
            var result = await sender.Send(command);
            if (result.IsSuccess)
            {
                //Thêm chi tiết cho response
                return Results.Ok(result);
            }

            return Results.BadRequest(result);
        }).RequireAuthorization("boss")
        .RequireRateLimiting("AntiSpamCreateCategoryCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        //Cập nhật category
        endpoints.MapPut("{id}",
        async (
            string id,
            [FromForm] string categoryName,
            [FromForm] IFormFile? categoryImage,
            ISender sender,
            HttpContext httpContext,
            IJwtProvider jwtProvider) =>
        {

            //lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);

            var command = new UpdateCategoryCommand(
                id, categoryName,
                categoryImage,
                token);

            var result = await sender.Send(command);
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).RequireAuthorization("boss")
        .RequireRateLimiting("AntiSpamUpdateCategoryCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        //Xóa category
        endpoints.MapDelete("{id}",
        async (
            string id,
            ISender sender,
            HttpContext httpContext,
            IJwtProvider jwtProvider) =>
        {
            //lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);

            var request = new RemoveCategoryCommand(id, token);
            var result = await sender.Send(request);
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);

        }).RequireAuthorization("boss")
        .RequireRateLimiting("AntiSpamRemoveCategoryCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();


        //Xóa nhiều category
        endpoints.MapDelete("", async (
        [FromForm] Ulid[] id,
        HttpContext httpContext,
        ISender sender,
        IJwtProvider jwtProvider) =>
        {
            // Lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);

            // Gửi command
            var result = await sender.Send(new RemoveManyCategoryCommand(id, token));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).RequireAuthorization("boss")
        .RequireRateLimiting("AntiSpamRemoveManyCategoryCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        //Khôi phục category
        endpoints.MapPut("restore/{id}", async (
            Ulid id,
            HttpContext httpContext,
            ISender sender,
            IJwtProvider jwtProvider) =>
        {
            // Lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);

            // Gửi command
            var result = await sender.Send(new RestoreCategoryCommand(id, token));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).RequireAuthorization("boss")
        .RequireRateLimiting("AntiSpamRestoreCategoryCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        //Khôi phục nhiều category
        endpoints.MapPut("restore", async (
        [FromForm] Ulid[] id,
        HttpContext httpContext,
        ISender sender,
        IJwtProvider jwtProvider) =>
        {
            // Lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);

            // Gửi command
            var result = await sender.Send(new RestoreManyCategoryCommand(id, token));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).RequireAuthorization("boss")
        .RequireRateLimiting("AntiSpamRestoreManyCategoryCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        //Lấy tên và id của category
        endpoints.MapGet("category-info",
        async (ISender sender) =>
        {
            //Gửi command
            var result = await sender.Send(new GetAllCategoryInfoQuery());
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);

        });

    }
}




