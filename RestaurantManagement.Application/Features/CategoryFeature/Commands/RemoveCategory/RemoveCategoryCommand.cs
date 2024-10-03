using MediatR;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.RemoveCategory;

public record RemoveCategoryCommand(Ulid Id) : IRequest<bool>;
 
 