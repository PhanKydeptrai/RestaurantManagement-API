using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.RestoreMeal;

public record RestoreMealCommand(string id, string token) : ICommand;
