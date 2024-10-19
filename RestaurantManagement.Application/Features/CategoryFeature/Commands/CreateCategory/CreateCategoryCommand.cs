using Microsoft.AspNetCore.Http;
using RestaurantManagement.Application.Abtractions;
namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.CreateCategory;

public record CreateCategoryCommand(string Name, IFormFile? Image, string Token) : ICommand;

