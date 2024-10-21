using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.RemoveMeal;

public record RemoveMealCommand(Ulid id, string token) : ICommand;
