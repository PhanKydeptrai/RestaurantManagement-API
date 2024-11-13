using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.BillDtos;

namespace RestaurantManagement.Application.Features.BillFeature.Queries.GetAllBillByUserId;

public record GetAllBillByUserIdQuery(
    // string? filter,
    string token,
    // string? searchTerm,
    string? sortColumn,
    string? sortOrder,
    int? page,
    int? pageSize
) : IQuery<PagedList<BillResponse>>;