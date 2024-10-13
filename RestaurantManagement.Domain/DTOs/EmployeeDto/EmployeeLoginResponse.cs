namespace RestaurantManagement.Domain.DTOs.EmployeeDto;

public record EmployeeLoginResponse(
    string UserId,
    string Email,
    string EmployeeStatus,
    string Role
);
