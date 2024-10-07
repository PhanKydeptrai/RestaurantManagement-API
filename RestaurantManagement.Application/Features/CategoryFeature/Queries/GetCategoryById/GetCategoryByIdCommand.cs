using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.CategoryDto;

namespace RestaurantManagement.Application.Features.CategoryFeature.Queries.GetCategoryById;

public record GetCategoryByIdCommand(Ulid Id) : IQuery<CategoryResponse>;
