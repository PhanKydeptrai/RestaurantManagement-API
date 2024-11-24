using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.ApplyVoucher;

public record ApplyVoucherRequest(string voucherCode, string phoneNumber);
public record ApplyVoucherCommand(string tableId, string voucherCode, string phoneNumber) : ICommand;
