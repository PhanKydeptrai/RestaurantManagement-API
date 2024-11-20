using System.Linq.Expressions;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetBillHistory;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.ActivityHistoryDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetTableTypeHistory;

public class GetTableTypeHistoryQueryHandler : IQueryHandler<GetTableTypeHistoryQuery, PagedList<ActivityHistoryResponse>>
{

    private readonly IApplicationDbContext _context;

    public GetTableTypeHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedList<ActivityHistoryResponse>>> Handle(GetTableTypeHistoryQuery request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new GetTableTypeHistoryQueryValidator();
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result<PagedList<ActivityHistoryResponse>>.Failure(errors!);
        }

        var tabletypeLogQuery = _context.TableTypeLogs.AsQueryable();

        //Filter
        if (!string.IsNullOrEmpty(request.filterUserId))
        {
            tabletypeLogQuery = tabletypeLogQuery.Where(x => x.UserId == Ulid.Parse(request.filterUserId));
        }

        //sort
        Expression<Func<TableTypeLog, object>> keySelector = request.sortColumn?.ToLower() switch
        {
            "tabletypelogid" => x => x.TableTypeLogId,
            _ => x => x.TableTypeLogId
        };

        if (request.sortOrder?.ToLower() == "desc")
        {
            tabletypeLogQuery = tabletypeLogQuery.OrderByDescending(keySelector);
        }
        else
        {
            tabletypeLogQuery = tabletypeLogQuery.OrderBy(keySelector);
        }

        //paged
        var billLogs = tabletypeLogQuery
            .Select(a => new ActivityHistoryResponse(
                a.TableTypeLogId,
                a.UserId,
                a.LogDetails,
                a.LogDate)).AsQueryable();
        var billLogList = await PagedList<ActivityHistoryResponse>.CreateAsync(billLogs, request.page ?? 1, request.pageSize ?? 10);

        return Result<PagedList<ActivityHistoryResponse>>.Success(billLogList);
    }
}
