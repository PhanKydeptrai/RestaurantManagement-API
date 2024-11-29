using RestaurantManagement.Domain.DTOs.OrderDto;

namespace RestaurantManagement.Domain.DTOs.BillDtos;

public record BillResponse(
    Ulid? UserId,
    string? LastName,
    string? FirstName,
    string? Email,
    string? PhoneNumber,
    int? TableId,
    Ulid BillId,
    DateTime CreatedDate,
    Ulid? BookingId,
    int? BookPrice,
    DateOnly? BookingDate,
    TimeOnly? BookingTime,
    Ulid? OrderId,
    int TotalPrice,
    string? PaymentType,
    OrderDetailResponse[] OrderDetails
);

