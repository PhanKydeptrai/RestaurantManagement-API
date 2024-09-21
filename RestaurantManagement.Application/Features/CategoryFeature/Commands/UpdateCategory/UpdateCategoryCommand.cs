using MediatR;
using RestaurantManagement.Domain.DTOs.Common;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.UpdateCategory;


public record UpdateCategoryCommand : IRequest<Result<bool>>
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string CategoryStatus { get; set; }
    public string Desciption { get; set; }


}
