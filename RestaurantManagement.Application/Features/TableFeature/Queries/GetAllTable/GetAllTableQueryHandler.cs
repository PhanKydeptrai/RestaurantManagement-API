using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.TableDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Queries.GetAllTable;

public class GetAllTableQueryHandler : IQueryHandler<GetAllTableQuery, PagedList<TableResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetAllTableQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedList<TableResponse>>> Handle(GetAllTableQuery request, CancellationToken cancellationToken)
    {
        var tableQuery = _context.Tables.Include(a => a.TableType).AsQueryable();
        //Search
        
        //Filter
        if(!string.IsNullOrEmpty(request.filterStatus))
        {
            tableQuery = tableQuery.Where(x => x.TableStatus == request.filterStatus);
        }

        //sort
        Expression<Func<Table, object>> keySelector = request.sortColumn?.ToLower() switch
        {
            "tableid" => x => x.TableId,
            _ => x => x.TableId
        };

        if (request.sortOrder?.ToLower() == "desc")
        {
            tableQuery = tableQuery.OrderByDescending(keySelector);
        }
        else
        {
            tableQuery = tableQuery.OrderBy(keySelector);
        }

        //paged
        var tables = tableQuery
            .Select(a => new TableResponse(
                a.TableId,
                a.TableType.TableTypeName,
                a.TableStatus))
                .AsQueryable();
        var tableList = await PagedList<TableResponse>.CreateAsync(tables, request.page ?? 1, request.pageSize ?? 10);

        return Result<PagedList<TableResponse>>.Success(tableList);
    }
}
