using System.Linq.Expressions;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.ActivityHistoryDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetMealHistory;

public class GetMealHistoryQueryHandler : IQueryHandler<GetMealHistoryQuery, PagedList<ActivityHistoryResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetMealHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedList<ActivityHistoryResponse>>> Handle(GetMealHistoryQuery request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new GetMealHistoryValidator();
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result<PagedList<ActivityHistoryResponse>>.Failure(errors!);
        }

        var mealLogQuery = _context.MealLogs.AsQueryable();

        //Filter
        if (!string.IsNullOrEmpty(request.filterUserId))
        {
            mealLogQuery = mealLogQuery.Where(x => x.UserId == Ulid.Parse(request.filterUserId));
        }

        //sort
        Expression<Func<MealLog, object>> keySelector = request.sortColumn?.ToLower() switch
        {
            "meallogid" => x => x.MealLogId,
            _ => x => x.MealLogId
        };

        if (request.sortOrder?.ToLower() == "desc")
        {
            mealLogQuery = mealLogQuery.OrderByDescending(keySelector);
        }
        else
        {
            mealLogQuery = mealLogQuery.OrderBy(keySelector);
        }

        //paged
        var customerLogs = mealLogQuery
            .Select(a => new ActivityHistoryResponse(
                a.MealLogId,
                a.UserId,
                a.LogDetails,
                a.LogDate)).AsQueryable();
        var customerLogList = await PagedList<ActivityHistoryResponse>.CreateAsync(customerLogs, request.page ?? 1, request.pageSize ?? 10);

        return Result<PagedList<ActivityHistoryResponse>>.Success(customerLogList);
    }
}

