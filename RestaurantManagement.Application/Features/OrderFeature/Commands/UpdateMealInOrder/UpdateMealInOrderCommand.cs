using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.UpdateMealInOrder;

public record UpdateMealInOrderCommand(
    Ulid OrderDetailId,
    int Quantity) : ICommand;

public record UpdateMealInOrderRequest(
    int Quantity);