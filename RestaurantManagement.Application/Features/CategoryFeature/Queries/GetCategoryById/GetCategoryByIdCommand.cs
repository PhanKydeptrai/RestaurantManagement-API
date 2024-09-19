using MediatR;
using RestaurantManagement.Application.Features.CategoryFeature.DTOs;
using RestaurantManagement.Domain.Response;

namespace RestaurantManagement.Application.Features.CategoryFeature.Queries.GetCategoryById;

public record GetCategoryByIdCommand(Guid Id) : IRequest<Result<CategoryResponse>>;
