namespace RestaurantManagement.Application.Features.CustomerFeature.DTOs;
public record CustomerResponse(
    Ulid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string? Gender,
    string UserStatus,
    string CustomerStatus,
    string CustomerType,
    string? UserImage);
