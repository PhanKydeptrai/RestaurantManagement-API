using MediatR;
using RestaurantManagement.Application.Features.CategoryFeature.CategoryFilter;

namespace RestaurantManagement.Application.Features.CategoryFeature.GetAllCategory;

public record GetAllCategoryQuery() : IRequest<List<CategoryResponse>>;
