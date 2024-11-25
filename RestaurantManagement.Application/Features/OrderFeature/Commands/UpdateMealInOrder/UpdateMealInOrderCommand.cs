using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.UpdateMealInOrder;

public record UpdateMealInOrderCommand(
    string OrderDetailId,
    object Quantity) : ICommand;

public record UpdateMealInOrderRequest(
    object Quantity);