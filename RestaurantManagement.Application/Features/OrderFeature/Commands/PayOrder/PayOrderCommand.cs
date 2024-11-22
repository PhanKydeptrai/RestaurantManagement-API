using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.PayOrder;

#region Old PayOrderCommand
public record PayOrderCommand(string tableId) : ICommand;
#endregion

#region New PayOrderCommand
// public record PayOrderRequest(string? voucherName, string? phoneNumber);

// public record PayOrderCommand(string tableId, string? voucherName, string? phoneNumber) : ICommand;
#endregion

