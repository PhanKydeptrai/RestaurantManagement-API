namespace RestaurantManagement.Domain.DTOs.CustomerDto;

public record CustomerLoginResponse(
    string UserId,
    string Email,
    string CustomerType,
    string UserStatus
);
