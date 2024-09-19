using MediatR;
using RestaurantManagement.Application.Features.CategoryFeature.Queries.CategoryFilter;
using RestaurantManagement.Domain.Response;

namespace RestaurantManagement.Application.Features.CategoryFeature.Queries.GetCategoryById;

public record GetCategoryByIdCommand(Guid Id) : IRequest<Result<CategoryResponse>>;
