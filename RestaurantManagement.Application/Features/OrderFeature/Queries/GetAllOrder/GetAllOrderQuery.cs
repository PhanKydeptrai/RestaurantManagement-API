using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.OrderDto;

namespace RestaurantManagement.Application.Features.OrderFeature.Queries.GetAllOrder;

public record GetAllOrderQuery(
    string? filterPaymentStatus,
    string? searchTerm,
    string? sortColumn,
    string? sortOrder,
    int? page,
    int? pageSize
) : IQuery<PagedList<OrderResponse>>;
