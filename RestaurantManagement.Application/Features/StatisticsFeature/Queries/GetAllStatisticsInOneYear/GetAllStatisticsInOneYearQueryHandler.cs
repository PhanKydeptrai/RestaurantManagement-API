using System.Globalization;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.DTOs.StatisticsDto;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.StatisticsFeature.Queries.GetAllStatisticsInOneYear;

public class GetAllStatisticsInOneYearQueryHandler : IQueryHandler<GetAllStatisticsInOneYearQuery, StatisticsResponseLite>
{
    private readonly IApplicationDbContext _context;

    public GetAllStatisticsInOneYearQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<StatisticsResponseLite>> Handle(GetAllStatisticsInOneYearQuery request, CancellationToken cancellationToken)
    {
        if(DateTime.TryParseExact(request.year, "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedYear))
        {
            var billQuery = _context.Bills.Where(a => a.CreatedDate.Year == parsedYear.Year).AsQueryable();
            StatisticsResponseLite statistics = await billQuery.AsNoTracking()
                .GroupBy(a => a.CreatedDate.Year)
                .Select(a => new StatisticsResponseLite(
                    a.Key.ToString(),
                    "VND",
                    a.GroupBy(b => b.CreatedDate.Month)
                        .Select(b => new StatisticsByMonthResponseLite(
                            b.Key.ToString(),
                            b.Sum(c => c.Total)))
                        .ToArray()))
                .FirstOrDefaultAsync();
            return Result<StatisticsResponseLite>.Success(statistics);
        }
        else
        {
            return Result<StatisticsResponseLite>.Failure(new[] { new Error("Year", "Invalid year format") });
        }
    }
}
