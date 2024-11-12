using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.CategoryDto;

namespace RestaurantManagement.Application.Features.CategoryFeature.Queries.GetCategoryById;

public record GetCategoryByIdQuery(Ulid Id) : IQuery<CategoryResponse>;
