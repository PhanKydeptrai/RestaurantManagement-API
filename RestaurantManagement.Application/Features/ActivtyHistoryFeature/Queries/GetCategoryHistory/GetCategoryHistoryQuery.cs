using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.ActivityHistoryDto;

namespace RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetCategoryHistory;
public record GetCategoryHistoryQuery(    
    string? filterUserId,
    string? searchTerm,
    string? sortColumn,
    string? sortOrder,
    int? page,
    int? pageSize) : IQuery<PagedList<ActivityHistoryResponse>>;