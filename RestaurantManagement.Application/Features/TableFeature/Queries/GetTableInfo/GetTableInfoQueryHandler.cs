using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.DTOs.TableDto;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Queries.GetTableInfo;

public class GetTableInfoQueryHandler(IApplicationDbContext context) : IQueryHandler<GetTableInfoQuery, TableInfo[]>
{
    public async Task<Result<TableInfo[]>> Handle(GetTableInfoQuery request, CancellationToken cancellationToken)
    {
        TableInfo[] tableInfos =  await context.Tables.Include(a => a.TableType)
            .Where(a => a.ActiveStatus == "Empty")
            .Select(a => new TableInfo(a.TableId, a.TableType.TableTypeName))
            .ToArrayAsync();
 
        return Result<TableInfo[]>.Success(tableInfos);
    }
}
