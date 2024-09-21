using MediatR;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.RemoveCategory;

public record RemoveCategoryCommand(Guid Id) : IRequest<bool>;
 
 