using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.ActivityHistoryDto;

namespace RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetTableTypeHistory;


public record GetTableTypeHistoryQuery(    
    string? filterUserId,
    string? searchTerm,
    string? sortColumn,
    string? sortOrder,
    int? page,
    int? pageSize) : IQuery<PagedList<ActivityHistoryResponse>>;
