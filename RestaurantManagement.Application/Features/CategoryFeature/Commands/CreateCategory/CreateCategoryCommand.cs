using MediatR;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.CreateCategory;

public record CreateCategoryCommand(string Name, string? Description, byte[]? Image) : IRequest<Result<bool>>;

