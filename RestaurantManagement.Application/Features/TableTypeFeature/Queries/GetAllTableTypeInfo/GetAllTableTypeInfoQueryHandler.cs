using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.TableTypeDto;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Queries.GetAllTableTypeInfo;

public class GetAllTableInfoQueryHandler : IQueryHandler<GetAllTableInfoQuery, List<TableTypeInfo>>
{

    private readonly ITableTypeRepository _tableTypeRepository;
    public GetAllTableInfoQueryHandler(ITableTypeRepository tableTypeRepository)
    {
        _tableTypeRepository = tableTypeRepository;
    }

    public async Task<Result<List<TableTypeInfo>>> Handle(GetAllTableInfoQuery request, CancellationToken cancellationToken)
    {
        var tableTypes = await _tableTypeRepository.GetAllTableTypeInfo(); 
        return Result<List<TableTypeInfo>>.Success(tableTypes);
    }
}
