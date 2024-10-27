using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.VoucherFeature.Commands.DeleteVoucher;

public record DeleteVoucherCommand(Ulid id, string token) : ICommand;
