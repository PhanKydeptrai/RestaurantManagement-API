using MediatR;
using RestaurantManagement.Domain.DTOs.Common;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.UpdateCategory;


public record UpdateCategoryCommand(
    Guid CategoryId, 
    string CategoryName, 
    string CategoryStatus, 
    string Desciption) : IRequest<Result<bool>>;
