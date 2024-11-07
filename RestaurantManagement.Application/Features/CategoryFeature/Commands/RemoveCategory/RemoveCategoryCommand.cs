using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.RemoveCategory;

public record RemoveCategoryCommand(string Id, string Token) : ICommand;

