namespace RestaurantManagement.Domain.DTOs.TableDto;
public record TableResponse(
    int TableId,
    string TableTypeName,
    string TableStatus,
    string ActiveStatus
);

