using MediatR;
using RestaurantManagement.Domain.DTOs.CategoryDto;
using RestaurantManagement.Domain.DTOs.Common;

namespace RestaurantManagement.Application.Features.CategoryFeature.Queries.GetCategoryById;

public record GetCategoryByIdCommand(Ulid Id) : IRequest<Result<CategoryResponse>>;
