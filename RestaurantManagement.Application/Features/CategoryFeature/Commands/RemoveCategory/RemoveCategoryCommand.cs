using MediatR;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.RemoveCategory;

public record RemoveCategoryCommand(Ulid Id) : IRequest<Result>;
 
 