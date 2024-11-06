using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.TableTypeDto;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Queries.GetAllTableTypeInfo;

public class GetAllTableInfoQueryHandler(ITableTypeRepository tableTypeRepository) : IQueryHandler<GetAllTableInfoQuery, List<TableTypeInfo>>
{
    public async Task<Result<List<TableTypeInfo>>> Handle(GetAllTableInfoQuery request, CancellationToken cancellationToken)
    {
        var tableTypes = await tableTypeRepository.GetAllTableTypeInfo(); 
        return Result<List<TableTypeInfo>>.Success(tableTypes);
    }
}
