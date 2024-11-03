namespace RestaurantManagement.Domain.DTOs.BookingDtos;

public record BookingResponse(
    Ulid BookId,
    Ulid UserId,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    DateOnly BookingDate,
    TimeOnly BookingTime,
    decimal BookingPrice,
    string PaymentStatus,
    int NumberOfCustomers,
    string? Note
);
