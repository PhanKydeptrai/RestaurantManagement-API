using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.API.Extentions;
using RestaurantManagement.Application.Features.CategoryFeature.Commands.CreateCategory;
using RestaurantManagement.Application.Features.CategoryFeature.Commands.RemoveCategory;
using RestaurantManagement.Application.Features.CategoryFeature.Commands.UpdateCategory;
using RestaurantManagement.Application.Features.CategoryFeature.Queries.CategoryFilter;
using RestaurantManagement.Application.Features.CategoryFeature.Queries.GetCategoryById;

namespace RestaurantManagement.API.Controllers;
public class CategoryController : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
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
                return Results.Ok(response.Value);
            }
            return Results.BadRequest(response.Errors);
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

            var command = new CreateCategoryCommand(name, imageData);

            var result = await sender.Send(command);
            if (result.IsSuccess)
            {
                return Results.Ok("Create successfully!");
            }
            
            return Results.BadRequest(result.ToProblemDetails());
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
            return Results.BadRequest(result.ToProblemDetails());
           
            
        });

        endpoints.MapDelete("{id}", async (Ulid id, ISender sender) =>
        {
            var request = new RemoveCategoryCommand(id);
            var result = await sender.Send(request);
            if (result.IsSuccess)
            {
                return Results.Ok("Remove successfully!");
            }
            return Results.BadRequest(result.ToProblemDetails());
        });


    }
    
    

}