using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.AddMealToOrder;

public record AddMealToOrderCommand(
    int TableId,
    Ulid MealId,
    int Quantity
) : ICommand;

public record AddMealToOrderRequest(
    string MealId,
    int Quantity
);