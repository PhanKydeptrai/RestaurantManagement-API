namespace RestaurantManagement.Domain.DTOs.StatisticsDto;

public record StatisticsByYearResponse(
    DateTime Date,
    string Currency,
    decimal TotalRevenue,
    StatisticsByMonthResponse[] StatisticsByMonthResponses
    
);

public record StatisticsByMonthResponse(
    DateTime Date,
    string Currency,
    decimal TotalRevenue,
    StatisticsByDayResponse[] StatisticsByDayResponses
);

public record StatisticsByDayResponse(
    DateTime Date,
    decimal TotalRevenue,
    string Currency
);