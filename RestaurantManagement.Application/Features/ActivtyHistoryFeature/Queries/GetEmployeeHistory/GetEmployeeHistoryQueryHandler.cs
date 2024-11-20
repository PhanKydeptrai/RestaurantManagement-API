using System.Linq.Expressions;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.ActivityHistoryDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetEmployeeHistory;

public class GetEmployeeHistoryQueryHandler : IQueryHandler<GetEmployeeHistoryQuery, PagedList<ActivityHistoryResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetEmployeeHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedList<ActivityHistoryResponse>>> Handle(GetEmployeeHistoryQuery request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new GetEmployeeHistoryQueryValidator();
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result<PagedList<ActivityHistoryResponse>>.Failure(errors!);
        }

        var customerLogQuery = _context.EmployeeLogs.AsQueryable();

        //Filter
        if (!string.IsNullOrEmpty(request.filterUserId))
        {
            customerLogQuery = customerLogQuery.Where(x => x.UserId == Ulid.Parse(request.filterUserId));
        }

        //sort
        Expression<Func<EmployeeLog, object>> keySelector = request.sortColumn?.ToLower() switch
        {
            "employeelogid" => x => x.EmployeeLogId,
            _ => x => x.EmployeeLogId
        };

        if (request.sortOrder?.ToLower() == "desc")
        {
            customerLogQuery = customerLogQuery.OrderByDescending(keySelector);
        }
        else
        {
            customerLogQuery = customerLogQuery.OrderBy(keySelector);
        }

        //paged
        var customerLogs = customerLogQuery
            .Select(a => new ActivityHistoryResponse(
                a.EmployeeLogId,
                a.UserId,
                a.LogDetails,
                a.LogDate)).AsQueryable();
        var customerLogList = await PagedList<ActivityHistoryResponse>.CreateAsync(customerLogs, request.page ?? 1, request.pageSize ?? 10);

        return Result<PagedList<ActivityHistoryResponse>>.Success(customerLogList);
    }
}
