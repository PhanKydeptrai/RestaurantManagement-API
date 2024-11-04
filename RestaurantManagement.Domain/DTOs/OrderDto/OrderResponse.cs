namespace RestaurantManagement.Domain.DTOs.OrderDto;

public record OrderResponse(
    int TableId,
    Ulid OrderId,
    string PaymentStatus,
    decimal Total,
    OrderDetailResponse[] OrderDetails
);

public record OrderDetailResponse(
    Ulid OrderDetailId,
    Ulid MealId,
    string MealName,
    string ImageUrl,
    int Quantity,
    decimal UnitPrice
);