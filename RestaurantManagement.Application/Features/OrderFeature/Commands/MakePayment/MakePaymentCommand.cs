
using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.MakePayment;

public record MakePaymentRequest(string? voucherCode, string? phoneNumber);

public record MakePaymentCommand(string tableId, string? voucherCode, string? phoneNumber) : ICommand;
