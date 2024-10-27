using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.VoucherFeature.Commands.UpdateVoucher;

public record UpdateVoucherCommand(
    Ulid VoucherId,
    string VoucherName, 
    decimal MaxDiscount, 
    decimal VoucherCondition,
    DateTime StartDate,
    DateTime ExpiredDate,
    string? Description,
    string token) : ICommand;

public record UpdateVoucherRequest(
    
    string VoucherName, 
    decimal MaxDiscount, 
    decimal VoucherCondition,
    DateTime StartDate,
    DateTime ExpiredDate,
    string? Description);