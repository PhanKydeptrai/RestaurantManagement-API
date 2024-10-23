using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.RestoreSellStatus;

public record RestoreSellStatusCommand(Ulid id, string token) : ICommand;
