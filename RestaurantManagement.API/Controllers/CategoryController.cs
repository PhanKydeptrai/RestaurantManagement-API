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
    //TODO:
    //Refactor, tạo phương thức lấy token từ HttpContext
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("api/category").WithTags("Category").DisableAntiforgery();


        //Lấy danh sách category
        endpoints.MapGet("", async (
            [FromQuery] string? seachTerm,
            [FromQuery] int page,
            [FromQuery] int pageSize, ISender sender) =>
        {
            var query = new CategoryFilterQuery(seachTerm, page, pageSize);
            var response = await sender.Send(query);
            return Results.Ok(response);

        });

        //Lấy category theo id
        endpoints.MapGet("{id}", async (Ulid id, ISender sender) =>
        {
            GetCategoryByIdCommand request = new GetCategoryByIdCommand(id);
            var response = await sender.Send(request);
            if (response.IsSuccess)
            {
                return Results.Ok(response.Value);
            }
            return Results.BadRequest(response.Errors);
        });


        //Tạo mới category
        endpoints.MapPost("", async (
            [FromForm] IFormFile? Image,
            [FromForm] string name,
            [FromForm] string? description,
            HttpContext httpContext,
            ISender sender,
            ISystemLogRepository systemLogRepository,
            IUnitOfWork unitOfWork) =>
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
            //lấy token
            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
            //Trích xuất token
            var token = authHeader.Substring("Bearer ".Length).Trim();

            //Gửi command
            var command = new CreateCategoryCommand(name, imageData, token);
            var result = await sender.Send(command);
            if (result.IsSuccess)
            {
                //Thêm chi tiết cho response
                return Results.Ok("Create successfully!");
            }

            return Results.BadRequest(result.ToProblemDetails());
        }).RequireAuthorization();

        //Cập nhật category
        endpoints.MapPut("{id}", async (
            Ulid id, 
            [FromForm]string categoryName,
            [FromForm]string categoryStatus,
            [FromForm]IFormFile? categoryImage,
            ISender sender,
            HttpContext httpContext) =>
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
            //lấy token
            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
            //Trích xuất token
            var token = authHeader.Substring("Bearer ".Length).Trim();

            var command = new UpdateCategoryCommand(
                id, categoryName,
                categoryStatus,
                imageData,
                token);

            var result = await sender.Send(command);
            if (result.IsSuccess)
            {
                return Results.Ok("Update successfully!");
            }
            return Results.BadRequest(result.ToProblemDetails());


        }).RequireAuthorization("Boss");

        //Xóa category
        endpoints.MapDelete("{id}", async (Ulid id, ISender sender, HttpContext httpContext) =>
        {
            //lấy token
            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
            //Trích xuất token
            var token = authHeader.Substring("Bearer ".Length).Trim();
            var request = new RemoveCategoryCommand(id,token);
            var result = await sender.Send(request);
            if (result.IsSuccess)
            {
                return Results.Ok("Remove successfully!");
            }
            return Results.BadRequest(result.ToProblemDetails());
        }).RequireAuthorization("Boss");


        //Xóa nhiều category
        endpoints.MapDelete("", async(
            [FromForm]Ulid[] id, 
            HttpContext httpContext,
            ISender sender) =>
        {
            
            //lấy token
            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
            //Trích xuất token
            var token = authHeader.Substring("Bearer ".Length).Trim();
            
            //Gửi command
            var result = await sender.Send(new RemoveManyCategoryCommand(id,token));
            if(result.IsSuccess)
            {
                return Results.Ok("Remove successfully!");
            }
            return Results.BadRequest(result.ToProblemDetails());

        }).RequireAuthorization("Boss");

        //Khôi phục category
        endpoints.MapPut ("restore/{id}", async (Ulid id, HttpContext httpContext, ISender sender) =>
        {

            //lấy token
            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
            //Trích xuất token
            var token = authHeader.Substring("Bearer ".Length).Trim();
            var result = await sender.Send(new RestoreCategoryCommand(id, token));
            if (result.IsSuccess)
            {
                return Results.Ok("Restore successfully!");
            }
            return Results.BadRequest(result.ToProblemDetails());
        }).RequireAuthorization("Boss");

        //Khôi phục nhiều category
        endpoints.MapPut("restore", async (
            [FromForm]Ulid[] id,
            HttpContext httpContext,
            ISender sender) =>
        {
            //lấy token
            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
            //Trích xuất token
            var token = authHeader.Substring("Bearer ".Length).Trim();
            //Gửi command
            var result = await sender.Send(new RestoreManyCategoryCommand(id, token));
            if(result.IsSuccess)
            {
               return Results.Ok("Restore successfully!");
            }
            return Results.BadRequest(result.ToProblemDetails());

        }).RequireAuthorization("Boss");

    }



}