using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.RestoreMeal;

public record RestoreMealCommand(Ulid id, string token) : ICommand;
