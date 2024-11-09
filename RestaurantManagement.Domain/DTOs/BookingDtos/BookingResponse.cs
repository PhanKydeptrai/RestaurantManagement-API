namespace RestaurantManagement.Domain.DTOs.BookingDtos;

public record BookingResponse(
    Ulid BookId,
    Ulid UserId,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    int? TableId,
    DateOnly BookingDate,
    TimeOnly BookingTime,
    decimal BookingPrice,
    string PaymentStatus,
    string BookingStatus,
    int NumberOfCustomers,
    string? Note
);
