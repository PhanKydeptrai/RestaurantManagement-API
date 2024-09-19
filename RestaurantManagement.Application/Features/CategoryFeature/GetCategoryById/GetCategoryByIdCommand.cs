using MediatR;
using RestaurantManagement.Application.Features.CategoryFeature.CategoryFilter;
using RestaurantManagement.Domain.Response;

namespace RestaurantManagement.Application.Features.CategoryFeature.GetCategoryById;

public record GetCategoryByIdCommand(Guid Id) : IRequest<Result<CategoryResponse>>;
