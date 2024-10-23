using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.ChangeSellStatus;

public record ChangeSellStatusCommand(Ulid id, string token) : ICommand;
