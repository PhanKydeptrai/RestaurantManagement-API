namespace RestaurantManagement.Domain.DTOs.CategoryDto;


public record CategoryResponse(
    Ulid CategoryId,
    string CategoryName,
    string CategoryStatus,
    string? ImageUrl);//NOTE: This will be changed to byte[] in the future
