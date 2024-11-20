using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.AddMealToOrder;

public record AddMealToOrderCommand(
    string TableId,
    string MealId,
    int Quantity,
    string Token
) : ICommand;

public record AddMealToOrderRequest(
    string MealId,
    int Quantity
);