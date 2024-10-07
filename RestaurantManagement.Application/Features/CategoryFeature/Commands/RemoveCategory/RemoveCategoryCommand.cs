using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.RemoveCategory;

public record RemoveCategoryCommand(Ulid Id) : ICommand;
 
 