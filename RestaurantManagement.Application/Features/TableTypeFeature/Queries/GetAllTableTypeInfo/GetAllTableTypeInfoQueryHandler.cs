using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.DTOs.TableTypeDto;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Queries.GetAllTableTypeInfo;

public class GetAllTableInfoQueryHandler : IQueryHandler<GetAllTableInfoQuery, List<TableTypeInfo>>
{
    private readonly IApplicationDbContext _context;

    public GetAllTableInfoQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<TableTypeInfo>>> Handle(GetAllTableInfoQuery request, CancellationToken cancellationToken)
    {
        var tableTypes = _context.TableTypes.Select(a => new TableTypeInfo(a.TableTypeId, a.TableTypeName)).ToList();
        return Result<List<TableTypeInfo>>.Success(tableTypes);
    }
}
