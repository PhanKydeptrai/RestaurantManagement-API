using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.PayOrder;


public record PayOrderCommand(string tableId) : ICommand;
