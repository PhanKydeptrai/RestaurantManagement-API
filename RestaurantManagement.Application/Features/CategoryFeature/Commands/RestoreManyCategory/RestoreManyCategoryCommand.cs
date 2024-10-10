using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.RestoreManyCategory;

public record RestoreManyCategoryCommand(Ulid[] id, string token) : ICommand;
