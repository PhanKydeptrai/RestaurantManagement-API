using RestaurantManagement.Application.Abtractions;
namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.CreateCategory;

public record CreateCategoryCommand(string Name, string? Image, string Token) : ICommand;

