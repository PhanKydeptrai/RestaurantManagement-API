using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.DTOs.TableDto;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Queries.GetTableById;

public class GetTableByIdQueryHandler : IQueryHandler<GetTableByIdQuery, TableResponse>
{
    private readonly ITableRepository _tableRepository;
    private readonly IApplicationDbContext _context;
    public GetTableByIdQueryHandler(
        ITableRepository tableRepository,
        IApplicationDbContext context)
    {
        _tableRepository = tableRepository;
        _context = context;
    }
    public async Task<Result<TableResponse>> Handle(GetTableByIdQuery request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new GetTableByIdQueryValidator(_tableRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result<TableResponse>.Failure(errors);
        }
        
        var table = await _context.Tables
            .Include(a => a.TableType).Where(a => a.TableId == request.id)
            .Select(a => new TableResponse(
                a.TableId,
                a.TableType.TableTypeName,
                a.TableStatus,
                a.ActiveStatus)).FirstOrDefaultAsync();

        return Result<TableResponse>.Success(table);
    }
}
