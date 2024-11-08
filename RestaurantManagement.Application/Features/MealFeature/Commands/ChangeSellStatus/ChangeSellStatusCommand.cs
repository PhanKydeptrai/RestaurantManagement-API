using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.ChangeSellStatus;

public record ChangeSellStatusCommand(string id, string token) : ICommand;
