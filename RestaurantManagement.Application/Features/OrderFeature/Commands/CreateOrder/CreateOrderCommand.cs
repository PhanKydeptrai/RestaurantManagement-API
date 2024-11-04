using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.CreateOrder;

public record CreateOrderCommand(
    int TableId,
    Ulid MealId,
    int Quantity
) : ICommand;

public record CreateOrderRequest(
    int TableId,
    string MealId,
    int Quantity
);