namespace RestaurantManagement.Domain.DTOs.ActivityHistoryDto;

public record ActivityHistoryResponse(
    Ulid UserLogId,
    Ulid UserId,
    string LogDetails,
    DateTime LogDate
);
