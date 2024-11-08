using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.RestoreSellStatus;

public record RestoreSellStatusCommand(string id, string token) : ICommand;
