using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.TableTypeDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;
using System.Linq.Expressions;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Queries.GetAllTableType;

public class GetAllTableTypeQueryHandler(IApplicationDbContext context) : IQueryHandler<GetAllTableTypeQuery, PagedList<TableTypeResponse>>
{
    public async Task<Result<PagedList<TableTypeResponse>>> Handle(GetAllTableTypeQuery request, CancellationToken cancellationToken)
    {
        var tableTypeQuery = context.TableTypes.AsQueryable();


        //Search
        if (!string.IsNullOrEmpty(request.searchTerm)) //tìm kiếm theo tên
        {
            tableTypeQuery = tableTypeQuery.Where(x => x.TableTypeName.Contains(request.searchTerm));
        }

        //Filter
        if (!string.IsNullOrEmpty(request.filterStatus)) //lọc theo trạng thái
        {
            tableTypeQuery = tableTypeQuery.Where(x => x.Status == request.filterStatus);
        }


        //sort

        Expression<Func<TableType, object>> keySelector = request.sortColumn?.ToLower() switch
        {
            //Các điều kiện
            "tabletypeid" => x => x.TableTypeId,
            _ => x => x.TableTypeId
        };

        if (request.sortOrder?.ToLower() == "desc")
        {
            tableTypeQuery = tableTypeQuery.OrderByDescending(keySelector);
        }
        else
        {
            tableTypeQuery = tableTypeQuery.OrderBy(keySelector);
        }

        //Paging    
        var tableTypes = tableTypeQuery
            .Select(a => new TableTypeResponse(
                a.TableTypeId,
                a.TableTypeName,
                a.Status,
                a.TableCapacity,
                a.ImageUrl,
                a.TablePrice,
                a.Description)).AsQueryable();
        var tableTypeList = await PagedList<TableTypeResponse>.CreateAsync(tableTypes, request.page ?? 1, request.pageSize ?? 10);

        return Result<PagedList<TableTypeResponse>>.Success(tableTypeList);
    }
}
