using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.RestoreCategory;

public record RestoreCategoryCommand(Ulid id, string token) : ICommand;
