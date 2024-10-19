using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.CreateMeal;

public record CreateMealCommand(
    string MealName,
    decimal Price,
    string? ImageUrl,
    string? Description,
    Ulid CategoryId,
    string token) : ICommand;

