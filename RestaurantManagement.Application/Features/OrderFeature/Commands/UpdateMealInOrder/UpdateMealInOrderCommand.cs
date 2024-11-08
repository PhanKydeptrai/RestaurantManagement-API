using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.UpdateMealInOrder;

public record UpdateMealInOrderCommand(
    string OrderDetailId,
    string Quantity) : ICommand;

public record UpdateMealInOrderRequest(
    string Quantity);