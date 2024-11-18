using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.StatisticsDto;

namespace RestaurantManagement.Application.Features.StatisticsFeature.Queries.GetStatisticsByDay;

public record GetStatisticsByDayQuery() : IQuery<StatisticsByDayResponse>;
// public record GetStatisticsByDayQuery(DateOnly Date) : IQuery<StatisticsByDayResponse>;

