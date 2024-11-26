

using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.PayOrderWithVnPay;

public record PayOrderWithVnPayCommand(string tableId) : ICommand<string>;
