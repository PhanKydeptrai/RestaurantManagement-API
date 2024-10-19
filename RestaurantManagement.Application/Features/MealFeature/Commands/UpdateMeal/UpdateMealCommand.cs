using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.UpdateMeal;

public record UpdateMealCommand(
    Ulid MealId,
    string MealName,
    decimal Price,
    string? ImageUrl,
    string? Description,
    Ulid CategoryId,
    string token) : ICommand;

