using Microsoft.AspNetCore.Http;
using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.UpdateMeal;

public record UpdateMealCommand(
    string MealId,
    string MealName,
    decimal Price,
    IFormFile? Image,
    string? Description,
    string CategoryId,
    string token) : ICommand;

