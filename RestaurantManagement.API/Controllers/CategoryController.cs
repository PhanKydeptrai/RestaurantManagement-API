using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.Features.CategoryFeature.Commands.CreateCategory;
using RestaurantManagement.Application.Features.CategoryFeature.Commands.RemoveCategory;
using RestaurantManagement.Application.Features.CategoryFeature.Commands.UpdateCategory;
using RestaurantManagement.Application.Features.CategoryFeature.Queries.CategoryFilter;
using RestaurantManagement.Application.Features.CategoryFeature.Queries.GetAllCategory;
using RestaurantManagement.Application.Features.CategoryFeature.Queries.GetCategoryById;
using RestaurantManagement.Domain.DTOs.Common;

namespace RestaurantManagement.API.Controllers;

[Route("api/category")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ISender _sender;

    public CategoryController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("search")] // api/category/search?searchTerm=abc&page=1&pageSize=10
    public async Task<IActionResult> Category(
        [FromQuery] string? searchTerm,
        [FromQuery] int page,
        [FromQuery] int pageSize)
    {
        var query = new CategoryFilterQuery(searchTerm, page, pageSize);
        var response = await _sender.Send(query);
        return Ok(response);
    }

    [HttpGet] // api/category
    public async Task<IActionResult> Categorys()
    {
        GetAllCategoryQuery query = new GetAllCategoryQuery();
        var response = await _sender.Send(query);
        return Ok(response);
    }

    [HttpGet("{id}")] // api/category/{id}
    public async Task<IActionResult> Category(Guid id)
    {
        GetCategoryByIdCommand command = new GetCategoryByIdCommand(id);
        var result = await _sender.Send(command);
        if (result.IsSuccess)
        {
            return Ok(result.ResultValue);
        }
        return NotFound("Category not found!");
    }


    [HttpPost] // api/category
    public async Task<IActionResult> Category(
        IFormFile? Image,
        [FromForm] string name,
        [FromForm] string? description)
    {
        byte[] imageData = null;
        if (Image != null)
        {
            
            using (var memoryStream = new MemoryStream())
            {
                await Image.CopyToAsync(memoryStream);
                imageData = memoryStream.ToArray();
            }
        }


        CreateCategoryCommand command = new CreateCategoryCommand
        {
            Name = name,
            Description = description,
            Image = imageData
        };

        Result<bool> result = await _sender.Send(command);
        if (result.IsSuccess)
        {
            return Ok("Create successfully!");
        }
        return BadRequest(result.Errors);

    }

    [HttpPut("{id}")] // api/category/
    public async Task<IActionResult> Category([FromRoute]Guid id, 
                                            [FromBody] UpdateCategoryRequest request)
    {
        UpdateCategoryCommand command = new UpdateCategoryCommand
        {
            CategoryId = id,
            CategoryName = request.CategoryName,
            CategoryStatus = request.CategoryStatus,
            Desciption = request.Desciption
        };

        var result = await _sender.Send(command);
        if (result.IsSuccess)
        {
            return Ok("Update successfully!");
        }
        return BadRequest(result.Errors);
    }

    [HttpDelete("{id}")] // api/category/{id}
    public async Task<IActionResult> RemoveCategory(Guid id)
    {
        RemoveCategoryCommand request = new RemoveCategoryCommand(id);
        var isSuccess = await _sender.Send(request);
        if(isSuccess)
        {
            return Ok("Remove successfully!");
        }
        return BadRequest("Remove failed! Please check category status");
    }

    

}