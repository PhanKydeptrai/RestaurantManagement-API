using MediatR;
using RestaurantManagement.Application.Features.CategoryFeature.Queries.CategoryFilter;

namespace RestaurantManagement.Application.Features.CategoryFeature.Queries.GetAllCategory;

public record GetAllCategoryQuery() : IRequest<List<CategoryResponse>>;
