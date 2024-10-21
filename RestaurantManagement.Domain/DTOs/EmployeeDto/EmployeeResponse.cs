namespace RestaurantManagement.Domain.DTOs.EmployeeDto;

public record EmployeeResponse(
    Ulid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string? Gender,
    string UserStatus,
    string EmployeeStatus,
    string Role,
    string? UserImage);