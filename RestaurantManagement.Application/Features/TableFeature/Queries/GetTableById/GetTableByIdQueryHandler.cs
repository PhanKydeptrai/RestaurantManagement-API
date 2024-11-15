using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.DTOs.TableDto;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Queries.GetTableById;

public class GetTableByIdQueryHandler(
    ITableRepository tableRepository,
    IApplicationDbContext context) : IQueryHandler<GetTableByIdQuery, TableResponse>
{
    public async Task<Result<TableResponse>> Handle(GetTableByIdQuery request, CancellationToken cancellationToken)
    {
        
        //Validate request
        var validator = new GetTableByIdQueryValidator(tableRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result<TableResponse>.Failure(errors!);
        }
        
        var table = await context.Tables
            .Include(a => a.TableType).Where(a => a.TableId == int.Parse(request.id))
            .Select(a => new TableResponse(
                a.TableId,
                a.TableType.TableTypeName,
                a.TableStatus,
                a.ActiveStatus))
                .FirstOrDefaultAsync();

        return Result<TableResponse>.Success(table);
    }
}
