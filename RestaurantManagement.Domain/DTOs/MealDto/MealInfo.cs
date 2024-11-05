namespace RestaurantManagement.Domain.DTOs.MealDto;

public record MealInfo(
    Ulid mealId, 
    string mealName, 
    decimal mealPrice,
    string? imageUrl);