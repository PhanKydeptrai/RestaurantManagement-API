using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.Features.CategoryFeature.CreateCategory;
using RestaurantManagement.Domain.Response;

namespace RestaurantManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ISender _sender;

    public CategoryController(ISender sender) => _sender = sender;

    [HttpPost]
    public async Task<IActionResult> Category(CreateCategoryCommand request)
    {
        Result<bool> result = await _sender.Send(request);
        if (result.IsSuccess)
        {
            return Ok("Create successfully!");
        }
        return BadRequest(result.Errors);

    }
}
