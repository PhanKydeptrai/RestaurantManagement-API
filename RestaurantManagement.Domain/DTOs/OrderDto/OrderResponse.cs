namespace RestaurantManagement.Domain.DTOs.OrderDto;

public record OrderResponse(
    int TableId,
    Ulid OrderId,
    string PaymentStatus,
    int Total,
    OrderDetailResponse[] OrderDetails
);

public record OrderDetailResponse(
    Ulid OrderDetailId,
    Ulid MealId,
    string MealName,
    int MealPrice,
    string ImageUrl,
    int Quantity,
    int UnitPrice
);