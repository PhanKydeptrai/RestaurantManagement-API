using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.CategoryDto;

namespace RestaurantManagement.Application.Features.CategoryFeature.Queries.GetAllCategoryId;

public record GetAllCategoryIdQuery() : IQuery<List<CategoryInfo>>;
