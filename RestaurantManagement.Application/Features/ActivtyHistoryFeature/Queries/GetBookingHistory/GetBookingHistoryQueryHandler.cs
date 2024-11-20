using System.Linq.Expressions;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.ActivityHistoryDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetBookingHistory;

public class GetBookingHistoryQueryHandler : IQueryHandler<GetBookingHistoryQuery, PagedList<ActivityHistoryResponse>>
{

    private readonly IApplicationDbContext _context;

    public GetBookingHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedList<ActivityHistoryResponse>>> Handle(GetBookingHistoryQuery request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new GetBookingHistoryQueryValidator();
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result<PagedList<ActivityHistoryResponse>>.Failure(errors!);
        }

        var bookingLogQuery = _context.BookingLogs.AsQueryable();

        //Filter
        if (!string.IsNullOrEmpty(request.filterUserId))
        {
            bookingLogQuery = bookingLogQuery.Where(x => x.UserId == Ulid.Parse(request.filterUserId));
        }

        //sort
        Expression<Func<BookingLog, object>> keySelector = request.sortColumn?.ToLower() switch
        {
            "bookinglogid" => x => x.BookingLogId,
            _ => x => x.BookingLogId
        };

        if (request.sortOrder?.ToLower() == "desc")
        {
            bookingLogQuery = bookingLogQuery.OrderByDescending(keySelector);
        }
        else
        {
            bookingLogQuery = bookingLogQuery.OrderBy(keySelector);
        }

        //paged
        var bookingLogs = bookingLogQuery
            .Select(a => new ActivityHistoryResponse(
                a.BookingLogId,
                a.UserId,
                a.LogDetails,
                a.LogDate)).AsQueryable();
        var bookingLogList = await PagedList<ActivityHistoryResponse>.CreateAsync(bookingLogs, request.page ?? 1, request.pageSize ?? 10);

        return Result<PagedList<ActivityHistoryResponse>>.Success(bookingLogList);
    }
}
