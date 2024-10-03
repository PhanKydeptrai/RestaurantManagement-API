using MediatR;
using RestaurantManagement.Domain.DTOs.Common;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.UpdateCategory;


public record UpdateCategoryCommand(
    Ulid CategoryId, 
    string CategoryName, 
    string CategoryStatus) : IRequest<Result<bool>>;
