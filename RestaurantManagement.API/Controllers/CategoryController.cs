using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.Features.CategoryFeature.Commands.CreateCategory;
using RestaurantManagement.Application.Features.CategoryFeature.Commands.RemoveCategory;
using RestaurantManagement.Application.Features.CategoryFeature.Commands.UpdateCategory;
using RestaurantManagement.Application.Features.CategoryFeature.Queries.CategoryFilter;
using RestaurantManagement.Application.Features.CategoryFeature.Queries.GetCategoryById;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.API.Controllers;
public static class CategoryController
{
    public static void MapCategoryEndpoint(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("api/category").WithTags("Category").DisableAntiforgery();

        endpoints.MapGet("", async (
            [FromQuery] string? seachTerm,
            [FromQuery] int page,
            [FromQuery] int pageSize, ISender sender) =>
        {
            var query = new CategoryFilterQuery(seachTerm, page, pageSize);
            var response = await sender.Send(query);
            return Results.Ok(response);
        });
        

        endpoints.MapGet("{id}", async (Ulid id, ISender sender) =>
        {
            GetCategoryByIdCommand request = new GetCategoryByIdCommand(id);
            var response = await sender.Send(request);
            if (response.IsSuccess)
            {
                return Results.Ok(response.ResultValue);
            }
            return Results.BadRequest("Category not found!");
        });
        
        endpoints.MapPost("", async (
            [FromForm] IFormFile? Image,
            [FromForm] string name,
            [FromForm] string? description, ISender sender) =>
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

            var command = new CreateCategoryCommand(name, description, imageData);

            Result<bool> result = await sender.Send(command);
            if (result.IsSuccess)
            {
                return Results.Ok("Create successfully!");
            }

            return Results.BadRequest(result.Errors);
        });

        endpoints.MapPut("{id}", async (Ulid id, UpdateCategoryRequest request, ISender sender) =>
        {
            var command = new UpdateCategoryCommand(
                id, request.CategoryName,
                request.CategoryStatus);
            var result = await sender.Send(command);
            if (result.IsSuccess)
            {
                return Results.Ok("Update successfully!");
            }
            return Results.BadRequest(result.Errors);
        });

        endpoints.MapDelete("{id}", async (Ulid id, ISender sender) =>
        {
            var request = new RemoveCategoryCommand(id);
            var isSuccess = await sender.Send(request);
            if (isSuccess)
            {
                return Results.Ok("Remove successfully!");
            }
            return Results.BadRequest("Remove failed! Please check category status");
        });
    }

    

}