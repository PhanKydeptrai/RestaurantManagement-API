using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.UpdateCategory;


public record UpdateCategoryCommand(
    Ulid CategoryId, 
    string CategoryName, 
    string CategoryStatus) : ICommand;
