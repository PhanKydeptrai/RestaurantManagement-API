using System.Linq.Expressions;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.ActivityHistoryDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetCategoryHistory;

public class GetCategoryHistoryQueryHandler : IQueryHandler<GetCategoryHistoryQuery, PagedList<ActivityHistoryResponse>>
{


    private readonly IApplicationDbContext _context;

    public GetCategoryHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedList<ActivityHistoryResponse>>> Handle(GetCategoryHistoryQuery request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new GetCategoryHistoryQueryValidator();
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result<PagedList<ActivityHistoryResponse>>.Failure(errors!);
        }

        var categoryLogQuery = _context.CategoryLogs.AsQueryable();

        //Filter
        if (!string.IsNullOrEmpty(request.filterUserId))
        {
            categoryLogQuery = categoryLogQuery.Where(x => x.UserId == Ulid.Parse(request.filterUserId));
        }

        //sort
        Expression<Func<CategoryLog, object>> keySelector = request.sortColumn?.ToLower() switch
        {
            "categorylogid" => x => x.CategoryLogId,
            _ => x => x.CategoryLogId
        };

        if (request.sortOrder?.ToLower() == "desc")
        {
            categoryLogQuery = categoryLogQuery.OrderByDescending(keySelector);
        }
        else
        {
            categoryLogQuery = categoryLogQuery.OrderBy(keySelector);
        }

        //paged
        var categoryLogs = categoryLogQuery
            .Select(a => new ActivityHistoryResponse(
                a.CategoryLogId,
                a.UserId,
                a.LogDetails,
                a.LogDate)).AsQueryable();
        var categoryLogList = await PagedList<ActivityHistoryResponse>.CreateAsync(categoryLogs, request.page ?? 1, request.pageSize ?? 10);

        return Result<PagedList<ActivityHistoryResponse>>.Success(categoryLogList);
    }
}
