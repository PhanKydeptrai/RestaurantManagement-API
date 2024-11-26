using RestaurantManagement.Domain.DTOs.OrderDto;

namespace RestaurantManagement.Domain.DTOs.MakePaymentDtos;

public record MakePaymentResponse(
    int TableId,
    Ulid OrderId,
    string PaymentStatus,
    int Total,
    string? VoucherCode,
    OrderDetailResponse[] OrderDetails
);