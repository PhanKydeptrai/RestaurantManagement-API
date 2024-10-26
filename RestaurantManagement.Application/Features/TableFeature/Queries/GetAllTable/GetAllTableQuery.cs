using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.TableDto;

namespace RestaurantManagement.Application.Features.TableFeature.Queries.GetAllTable;

public record GetAllTableQuery(
    string? filterTableType,
    string? filterStatus,
    string? searchTerm,
    string? sortColumn,
    string? sortOrder,
    int? page,
    int? pageSize) : IQuery<PagedList<TableResponse>>;

