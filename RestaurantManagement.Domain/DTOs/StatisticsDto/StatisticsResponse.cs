namespace RestaurantManagement.Domain.DTOs.StatisticsDto;

public record StatisticsByDayResponse(
    DateTime Date,
    decimal TotalRevenue
);

public record StatisticsByMonthResponse(
    DateTime Date,
    decimal TotalRevenue,
    StatisticsByDayResponse[]? StatisticsByDayResponses
);

public record StatisticsResponse(
    string Year,
    decimal TotalRevenue,
    string Currency,
    StatisticsByMonthResponse[]? StatisticsByMonthResponses
);

