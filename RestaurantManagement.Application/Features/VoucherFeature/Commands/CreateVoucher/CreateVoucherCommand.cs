using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.VoucherFeature.Commands.CreateVoucher;

//TODO: Cần xử lý unstrusted data
//TODO: Xử lý xác định voucherType

#region New CreateVoucherCommand
public record CreateVoucherCommand(
    string VoucherName,
    string VoucherCode,
    int? PercentageDiscount,
    decimal MaximumDiscountAmount,
    decimal MinimumOrderAmount,
    int? VoucherConditions,
    string StartDate,
    string ExpiredDate,
    string? Description,
    string token
) : ICommand;

public record CreateVoucherRequest(
    string VoucherName,
    string VoucherCode,
    int? PercentageDiscount,
    decimal MaximumDiscountAmount,
    decimal MinimumOrderAmount,
    int? VoucherConditions,
    string StartDate,
    string ExpiredDate,
    string? Description
);
#endregion

