using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.DTOs.TableTypeDto;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Queries.GetTableTypeById;

public class GetTableTypyByIdQueryHandler(ITableTypeRepository tableTypeRepository) : IQueryHandler<GetTableTypyByIdQuery, TableTypeResponse>
{
    public async Task<Result<TableTypeResponse>> Handle(GetTableTypyByIdQuery request, CancellationToken cancellationToken)
    {
        
        //Validate request
        var validator = new GetTableTypyByIdQueryValidator();
        Error[]? error = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out error));
        if (!isValid)
        {
            return Result<TableTypeResponse>.Failure(error!);
        }

        var tableType = await tableTypeRepository.GetTableTypeById(Ulid.Parse(request.id));
        if (tableType != null)
        {
            return Result<TableTypeResponse>.Success(tableType);
        }
        var errors = new[] { new Error("Table Type", $"Table Type with id {request.id} not found") };
        return Result<TableTypeResponse>.Failure(errors);
    }
}
