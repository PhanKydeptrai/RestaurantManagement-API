namespace RestaurantManagement.Domain.DTOs.VoucherDto;

public record VoucherDto(
    Ulid VoucherId, 
    string VoucherName, 
    string VoucherCode, 
    string VoucherType, 
    int? PercentageDiscount, 
    decimal MaximumDiscountAmount, 
    decimal MinimumOrderAmount, 
    decimal VoucherConditions, 
    string StartDate, 
    string ExpiredDate, 
    string? Description, 
    string Status
);

