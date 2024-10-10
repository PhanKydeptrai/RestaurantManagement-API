using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.RemoveManyCategory;

public record RemoveManyCategoryCommand(Ulid[] id, string Token) : ICommand;

