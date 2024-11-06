using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.TableTypeDto;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Queries.GetTableTypeById;

public class GetTableTypyByIdQueryHandler(ITableTypeRepository tableTypeRepository) : IQueryHandler<GetTableTypyByIdQuery, TableTypeResponse>
{
    public async Task<Result<TableTypeResponse>> Handle(GetTableTypyByIdQuery request, CancellationToken cancellationToken)
    {
        var tableType = await tableTypeRepository.GetTableTypeById(request.id);
        if(tableType != null)
        {
            return Result<TableTypeResponse>.Success(tableType);
        }
        var errors = new[] { new Error("Table Type", $"Table Type with id {request.id} not found") };
        return Result<TableTypeResponse>.Failure(errors);
    }
}
