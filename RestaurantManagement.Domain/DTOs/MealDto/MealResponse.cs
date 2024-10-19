namespace RestaurantManagement.Domain.DTOs.MealDto;

public record MealResponse(
    Ulid MealId,
    string MealName,
    decimal Price,
    string? ImageUrl,
    string? Description,
    string SellStatus,
    string MealStatus,
    string CategoryName);
