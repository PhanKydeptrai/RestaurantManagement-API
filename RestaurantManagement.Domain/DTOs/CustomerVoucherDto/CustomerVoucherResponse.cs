namespace RestaurantManagement.Domain.DTOs.CustomerVoucherDto;

public record CustomerVoucherResponse(
    Ulid VoucherId,
    string VoucherName,
    string VoucherCode,
    string VoucherType,
    int? PercentageDiscount,
    decimal MaximumDiscountAmount,
    decimal MinimumOrderAmount,
    decimal VoucherConditions,
    DateTime StartDate,
    DateTime ExpiredDate,
    string? Description,
    string Status,
    int Quantity
);


