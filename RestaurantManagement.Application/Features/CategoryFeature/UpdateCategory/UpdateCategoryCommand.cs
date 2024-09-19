using MediatR;
using RestaurantManagement.Domain.Response;

namespace RestaurantManagement.Application.Features.CategoryFeature.UpdateCategory;


public record UpdateCategoryCommand : IRequest<Result<bool>>
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string CategoryStatus { get; set; }
    public string Desciption { get; set; }

    
}
