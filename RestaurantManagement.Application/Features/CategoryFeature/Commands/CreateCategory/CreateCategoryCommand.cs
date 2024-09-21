using MediatR;
using RestaurantManagement.Domain.DTOs.Common;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.CreateCategory;

public class CreateCategoryCommand(string Name, string? Desciption, byte[]? Image) : IRequest<Result<bool>>;

