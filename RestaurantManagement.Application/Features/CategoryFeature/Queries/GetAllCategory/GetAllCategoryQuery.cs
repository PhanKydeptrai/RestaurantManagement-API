using MediatR;
using RestaurantManagement.Domain.DTOs.CategoryDto;

namespace RestaurantManagement.Application.Features.CategoryFeature.Queries.GetAllCategory;

public record GetAllCategoryQuery() : IRequest<List<CategoryResponse>>;
