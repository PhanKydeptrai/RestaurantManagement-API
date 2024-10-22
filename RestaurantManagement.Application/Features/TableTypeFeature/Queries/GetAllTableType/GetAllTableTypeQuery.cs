using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.TableTypeDto;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Queries.GetAllTableType;

public record GetAllTableTypeQuery(
    string? searchTerm,
    string? filterStatus,
    string? sortColumn,
    string? sortOrder,
    int? page,
    int? pageSize) : IQuery<PagedList<TableTypeResponse>>;

