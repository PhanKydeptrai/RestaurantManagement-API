using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.StatisticsDto;

namespace RestaurantManagement.Application.Features.StatisticsFeature.Queries.GetAllStatisticsInOneYear;

public record GetAllStatisticsInOneYearQuery(string year) : IQuery<StatisticsResponseLite>;
