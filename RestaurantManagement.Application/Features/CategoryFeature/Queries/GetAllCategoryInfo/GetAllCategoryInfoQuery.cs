using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.CategoryDto;

namespace RestaurantManagement.Application.Features.CategoryFeature.Queries.GetAllCategoryInfo;

public record GetAllCategoryInfoQuery() : IQuery<List<CategoryInfo>>;
