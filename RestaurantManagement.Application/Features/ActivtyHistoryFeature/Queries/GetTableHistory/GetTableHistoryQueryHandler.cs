using System.Linq.Expressions;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.ActivityHistoryDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetTableHistory;

public class GetTableHistoryQueryHandler : IQueryHandler<GetTableHistoryQuery, PagedList<ActivityHistoryResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetTableHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedList<ActivityHistoryResponse>>> Handle(GetTableHistoryQuery request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new GetTableHistoryQueryValidator();
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result<PagedList<ActivityHistoryResponse>>.Failure(errors!);
        }

        var tableLogQuery = _context.TableLogs.AsQueryable();

        //Filter
        if (!string.IsNullOrEmpty(request.filterUserId))
        {
            tableLogQuery = tableLogQuery.Where(x => x.UserId == Ulid.Parse(request.filterUserId));
        }

        //sort
        Expression<Func<TableLog, object>> keySelector = request.sortColumn?.ToLower() switch
        {
            "tablelogid" => x => x.TableLogId,
            _ => x => x.TableLogId
        };

        if (request.sortOrder?.ToLower() == "desc")
        {
            tableLogQuery = tableLogQuery.OrderByDescending(keySelector);
        }
        else
        {
            tableLogQuery = tableLogQuery.OrderBy(keySelector);
        }

        //paged
        var billLogs = tableLogQuery
            .Select(a => new ActivityHistoryResponse(
                a.TableLogId,
                a.UserId,
                a.LogDetails,
                a.LogDate)).AsQueryable();
        var billLogList = await PagedList<ActivityHistoryResponse>.CreateAsync(billLogs, request.page ?? 1, request.pageSize ?? 10);

        return Result<PagedList<ActivityHistoryResponse>>.Success(billLogList);
    }
}
