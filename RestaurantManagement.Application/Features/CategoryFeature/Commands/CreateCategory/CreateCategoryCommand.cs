using MediatR;
using RestaurantManagement.Domain.DTOs.Common;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.CreateCategory;

public class CreateCategoryCommand : IRequest<Result<bool>>
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public byte[]? Image { get; set; }
}
