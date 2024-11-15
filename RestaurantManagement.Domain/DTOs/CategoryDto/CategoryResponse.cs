namespace RestaurantManagement.Domain.DTOs.CategoryDto;


public record CategoryResponse(
    Ulid CategoryId,
    string CategoryName,
    string CategoryStatus,
    string? ImageUrl);
