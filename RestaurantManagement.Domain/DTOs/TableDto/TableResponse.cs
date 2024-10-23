namespace RestaurantManagement.Domain.DTOs.TableDto;
public record TableResponse(
    Ulid TableId,
    string TableTypeName,
    string TableStatus
);

