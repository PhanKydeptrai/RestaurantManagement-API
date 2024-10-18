using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.API.Extentions;
using RestaurantManagement.Application.Features.CategoryFeature.Commands.CreateCategory;
using RestaurantManagement.Application.Features.CategoryFeature.Commands.RemoveCategory;
using RestaurantManagement.Application.Features.CategoryFeature.Commands.RemoveManyCategory;
using RestaurantManagement.Application.Features.CategoryFeature.Commands.RestoreCategory;
using RestaurantManagement.Application.Features.CategoryFeature.Commands.RestoreManyCategory;
using RestaurantManagement.Application.Features.CategoryFeature.Commands.UpdateCategory;
using RestaurantManagement.Application.Features.CategoryFeature.Queries.CategoryFilter;
using RestaurantManagement.Application.Features.CategoryFeature.Queries.GetCategoryById;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.API.Controllers;
public class CategoryController : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("api/category").WithTags("Category").DisableAntiforgery();


        //Lấy danh sách category
        endpoints.MapGet("", 
        async (
            [FromQuery] string? seachTerm,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int page,
            [FromQuery] int pageSize, ISender sender) =>
        {
            var query = new CategoryFilterQuery(seachTerm,sortColumn, sortOrder, page, pageSize);
            var response = await sender.Send(query);
            return Results.Ok(response);

        });

        //Lấy category theo id
        endpoints.MapGet("{id}", async (Ulid id, ISender sender) =>
        {
            var response = await sender.Send(new GetCategoryByIdCommand(id));
            if (response.IsSuccess)
            {
                return Results.Ok(response);
            }
            return Results.BadRequest(response.ToProblemDetails());
        });


        //Tạo mới category
        endpoints.MapPost("", 
        async (
            [FromForm] IFormFile? Image,
            [FromForm] string name,
            [FromForm] string? description,
            HttpContext httpContext,
            ISender sender,
            ISystemLogRepository systemLogRepository,
            IUnitOfWork unitOfWork,
            IJwtProvider jwtProvider) =>
        {
            byte[] imageData = null!;
            if (Image != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await Image.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                }
            }

            var token = jwtProvider.GetTokenFromHeader(httpContext);

            //Gửi command
            var command = new CreateCategoryCommand(name, imageData, token);
            var result = await sender.Send(command);
            if (result.IsSuccess)
            {
                //Thêm chi tiết cho response
                return Results.Ok(result);
            }

            return Results.BadRequest(result.ToProblemDetails());
        });

        //Cập nhật category
        endpoints.MapPut("{id}",
        async (
            Ulid id,
            [FromForm] string categoryName,
            [FromForm] string categoryStatus,
            [FromForm] IFormFile? categoryImage,
            ISender sender,
            HttpContext httpContext,
            IJwtProvider jwtProvider) =>
        {
            byte[] imageData = null!;
            if (categoryImage != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await categoryImage.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                }
            }
            //1lấy token
            
            var token = jwtProvider.GetTokenFromHeader(httpContext);

            var command = new UpdateCategoryCommand(
                id, categoryName,
                categoryStatus,
                imageData,
                token);

            var result = await sender.Send(command);
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result.ToProblemDetails());
        });

        //Xóa category
        endpoints.MapDelete("{id}", 
        async (
            Ulid id, 
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
            return Results.BadRequest(result.ToProblemDetails());
            
        });


        //Xóa nhiều category
        endpoints.MapDelete("", 
        async (
            [FromForm] Ulid[] id,
            HttpContext httpContext,
            ISender sender,
            IJwtProvider jwtProvider) =>
        {

            //lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);

            //Gửi command
            var result = await sender.Send(new RemoveManyCategoryCommand(id, token));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result.ToProblemDetails());

        });

        //Khôi phục category
        endpoints.MapPut("restore/{id}", 
        async (
            Ulid id, 
            HttpContext 
            httpContext, 
            ISender sender,
            IJwtProvider jwtProvider) =>
        {

            //lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);

            var result = await sender.Send(new RestoreCategoryCommand(id, token));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result.ToProblemDetails());
        });

        //Khôi phục nhiều category
        endpoints.MapPut("restore", 
        async (
            [FromForm] Ulid[] id,
            HttpContext httpContext,
            ISender sender,
            IJwtProvider jwtProvider) =>
        {
            //lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);

            //Gửi command
            var result = await sender.Send(new RestoreManyCategoryCommand(id, token));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result.ToProblemDetails());

        });

    }



}