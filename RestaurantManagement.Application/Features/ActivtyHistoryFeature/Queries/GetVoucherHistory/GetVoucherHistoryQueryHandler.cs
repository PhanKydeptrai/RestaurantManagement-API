using System.Linq.Expressions;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.ActivityHistoryDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetVoucherHistory;

public class GetVoucherHistoryQueryHandler : IQueryHandler<GetVoucherHistoryQuery, PagedList<ActivityHistoryResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetVoucherHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedList<ActivityHistoryResponse>>> Handle(GetVoucherHistoryQuery request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new GetVoucherHistoryQueryValidator();
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result<PagedList<ActivityHistoryResponse>>.Failure(errors!);
        }

        var voucherLogQuery = _context.VoucherLogs.AsQueryable();

        //Filter
        if (!string.IsNullOrEmpty(request.filterUserId))
        {
            voucherLogQuery = voucherLogQuery.Where(x => x.UserId == Ulid.Parse(request.filterUserId));
        }

        //sort
        Expression<Func<VoucherLog, object>> keySelector = request.sortColumn?.ToLower() switch
        {
            "voucherlogid" => x => x.VoucherLogId,
            _ => x => x.VoucherLogId
        };

        if (request.sortOrder?.ToLower() == "desc")
        {
            voucherLogQuery = voucherLogQuery.OrderByDescending(keySelector);
        }
        else
        {
            voucherLogQuery = voucherLogQuery.OrderBy(keySelector);
        }

        //paged
        var voucherLogs = voucherLogQuery
            .Select(a => new ActivityHistoryResponse(
                a.VoucherLogId,
                a.UserId,
                a.LogDetails,
                a.LogDate)).AsQueryable();
        var voucherLogList = await PagedList<ActivityHistoryResponse>.CreateAsync(voucherLogs, request.page ?? 1, request.pageSize ?? 10);

        return Result<PagedList<ActivityHistoryResponse>>.Success(voucherLogList);
    }
}
