using System.Globalization;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.DTOs.StatisticsDto;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.StatisticsFeature.Queries.GetStatisticsByDay;

public class GetStatisticsByDayQueryHandler : IQueryHandler<GetStatisticsByDayQuery, StatisticsResponse>
{
    private readonly IApplicationDbContext _context;

    public GetStatisticsByDayQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<StatisticsResponse>> Handle(GetStatisticsByDayQuery request, CancellationToken cancellationToken)
    {


        if (DateTime.TryParseExact(request.datetime, "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedYear))
        {
            var billQuery = _context.Bills.Where(a => a.CreatedDate.Year == parsedYear.Year).AsQueryable();
            
         

            StatisticsResponse statistics = await billQuery.AsNoTracking()
                .GroupBy(a => a.CreatedDate.Year)
                .Select(a => new StatisticsResponse(
                    a.Key.ToString(), 
                    a.Sum(b => b.Total), 
                    "VND", 
                    a.GroupBy(b => b.CreatedDate.Month) 
                        .Select(b => new StatisticsByMonthResponse(
                            new DateTime(a.Key, b.Key, 1),
                            b.Sum(c => c.Total),
                            b.GroupBy(c => c.CreatedDate.Day)
                                .Select(c => new StatisticsByDayResponse(
                                    new DateTime(a.Key, b.Key, c.Key),
                                    c.Sum(d => d.Total)))
                            .ToArray()))
                            .ToArray()))
                            .FirstOrDefaultAsync();

            return Result<StatisticsResponse>.Success(statistics);
        }
        else
        {
            return Result<StatisticsResponse>.Failure(new[] { new Error("date", "Invalid date") });
        }

    }


}
