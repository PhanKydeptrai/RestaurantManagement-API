using MediatR;
using RestaurantManagement.Domain.Response;

namespace RestaurantManagement.Application.Features.CategoryFeature.CreateCategory;

public class CreateCategoryCommand : IRequest<Result<bool>>
{
    public string Name { get; set;}
}
