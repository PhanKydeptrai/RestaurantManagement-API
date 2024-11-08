using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.RemoveMeal;

public record RemoveMealCommand(string id, string token) : ICommand;
