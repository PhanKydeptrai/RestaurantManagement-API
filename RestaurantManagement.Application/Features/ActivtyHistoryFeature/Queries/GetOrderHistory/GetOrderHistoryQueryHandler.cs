using System.Linq.Expressions;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetMealHistory;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.ActivityHistoryDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetOrderHistory;

public class GetOrderHistoryQueryHandler : IQueryHandler<GetOrderHistoryQuery, PagedList<ActivityHistoryResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetOrderHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedList<ActivityHistoryResponse>>> Handle(GetOrderHistoryQuery request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new GetOrderHistoryQueryValidator();
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result<PagedList<ActivityHistoryResponse>>.Failure(errors!);
        }

        var orderLogsQuery = _context.OrderLogs.AsQueryable();

        //Filter
        if (!string.IsNullOrEmpty(request.filterUserId))
        {
            orderLogsQuery = orderLogsQuery.Where(x => x.UserId == Ulid.Parse(request.filterUserId));
        }

        //sort
        Expression<Func<OrderLog, object>> keySelector = request.sortColumn?.ToLower() switch
        {
            "orderlogid" => x => x.OrderLogId,
            _ => x => x.OrderLogId
        };

        if (request.sortOrder?.ToLower() == "desc")
        {
            orderLogsQuery = orderLogsQuery.OrderByDescending(keySelector);
        }
        else
        {
            orderLogsQuery = orderLogsQuery.OrderBy(keySelector);
        }

        //paged
        var customerLogs = orderLogsQuery
            .Select(a => new ActivityHistoryResponse(
                a.OrderLogId,
                a.UserId,
                a.LogDetails,
                a.LogDate)).AsQueryable();

        var customerLogList = await PagedList<ActivityHistoryResponse>.CreateAsync(customerLogs, request.page ?? 1, request.pageSize ?? 10);

        return Result<PagedList<ActivityHistoryResponse>>.Success(customerLogList);
    }
}
