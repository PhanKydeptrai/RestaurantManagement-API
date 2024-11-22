namespace RestaurantManagement.Domain.DTOs.StatisticsDto;

#region StatisticsResponse
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
#endregion

#region StatisticsResponse minimized just for the purpose of the test
//Get 12 months of statistics in a year
public record StatisticsResponseLite(
    string Year,
    string Currency,
    StatisticsByMonthResponseLite[]? StatisticsByMonthResponses
);


public record StatisticsByMonthResponseLite(
    string month,
    decimal TotalRevenue
);
#endregion
