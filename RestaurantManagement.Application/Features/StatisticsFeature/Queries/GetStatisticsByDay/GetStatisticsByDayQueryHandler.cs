using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.DTOs.StatisticsDto;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.StatisticsFeature.Queries.GetStatisticsByDay;

public class GetStatisticsByDayQueryHandler : IQueryHandler<GetStatisticsByDayQuery, StatisticsByDayResponse>
{
    private readonly IApplicationDbContext _context;

    public GetStatisticsByDayQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<StatisticsByDayResponse>> Handle(GetStatisticsByDayQuery request, CancellationToken cancellationToken)
    {
        decimal statistics = await _context.Bills.Where(b => b.CreatedDate == DateTime.Now.Date)
            .SumAsync(a => a.Total);
            
        return Result<StatisticsByDayResponse>.Success(new StatisticsByDayResponse(DateTime.Now.Date, statistics, "VNĐ"));
    }
}
