namespace RestaurantManagement.Domain.DTOs.MealDto;

public record MealResponse(
    Ulid MealId, 
    string MealName, 
    decimal Price,
    byte[]? Image,
    string? Description,
    string SellStatus,
    string MealStatus,
    string CategoryName);
