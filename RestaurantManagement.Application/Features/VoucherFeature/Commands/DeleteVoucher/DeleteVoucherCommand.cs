using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.VoucherFeature.Commands.DeleteVoucher;

public record DeleteVoucherCommand(string id, string token) : ICommand;
