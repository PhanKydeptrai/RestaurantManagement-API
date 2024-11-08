using Microsoft.AspNetCore.Http;
using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.CreateMeal;

public record CreateMealCommand(
    string MealName,
    decimal Price,
    IFormFile? Image,
    string? Description,
    string CategoryId,
    string token) : ICommand;

