using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.VoucherFeature.Commands.CreateVoucher;

#region New CreateVoucherCommand
public record CreateVoucherCommand(
    string VoucherName,
    string VoucherCode,
    object? PercentageDiscount,
    string MaximumDiscountAmount,
    string MinimumOrderAmount,
    object? VoucherConditions,
    DateTime StartDate,
    DateTime ExpiredDate,
    string? Description,
    string token
) : ICommand;

public record CreateVoucherRequest(
    string VoucherName,
    string VoucherCode,
    object? PercentageDiscount,
    string MaximumDiscountAmount,
    string MinimumOrderAmount,
    object? VoucherConditions,
    DateTime StartDate,
    DateTime ExpiredDate,
    string? Description
);
#endregion




#region OldCreateVoucherCommand
// public record CreateVoucherCommand(
// string VoucherName,
// decimal MaxDiscount,
// decimal VoucherCondition,
// DateTime StartDate,
// DateTime ExpiredDate,
// string? Description,
// string token) : ICommand;

// public record CreateVoucherRequest(
//     string VoucherName,
//     decimal MaxDiscount,
//     decimal VoucherCondition,
//     DateTime StartDate,
//     DateTime ExpiredDate,
//     string? Description);
#endregion