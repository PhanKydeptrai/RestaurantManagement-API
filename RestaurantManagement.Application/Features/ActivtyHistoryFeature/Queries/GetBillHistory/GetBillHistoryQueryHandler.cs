using System.Linq.Expressions;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.ActivityHistoryDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetBillHistory;

public class GetBillHistoryQueryHandler : IQueryHandler<GetBillHistoryQuery, PagedList<ActivityHistoryResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetBillHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedList<ActivityHistoryResponse>>> Handle(GetBillHistoryQuery request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new GetBillHistoryQueryValidator();
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result<PagedList<ActivityHistoryResponse>>.Failure(errors!);
        }

        var billLogQuery = _context.BillLogs.AsQueryable();

        //Filter
        if (!string.IsNullOrEmpty(request.filterUserId))
        {
            billLogQuery = billLogQuery.Where(x => x.UserId == Ulid.Parse(request.filterUserId));
        }

        //sort
        Expression<Func<BillLog, object>> keySelector = request.sortColumn?.ToLower() switch
        {
            "billlogid" => x => x.BillLogId,
            _ => x => x.BillLogId
        };

        if (request.sortOrder?.ToLower() == "desc")
        {
            billLogQuery = billLogQuery.OrderByDescending(keySelector);
        }
        else
        {
            billLogQuery = billLogQuery.OrderBy(keySelector);
        }

        //paged
        var billLogs = billLogQuery
            .Select(a => new ActivityHistoryResponse(
                a.BillLogId,
                a.UserId,
                a.LogDetails,
                a.LogDate)).AsQueryable();
        var billLogList = await PagedList<ActivityHistoryResponse>.CreateAsync(billLogs, request.page ?? 1, request.pageSize ?? 10);

        return Result<PagedList<ActivityHistoryResponse>>.Success(billLogList);
    }
}
