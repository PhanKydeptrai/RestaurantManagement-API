using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.CustomerVoucherDto;

namespace RestaurantManagement.Application.Features.CustomerFeature.Queries.GetVoucherOfCustomerByUserId;

public record GetVoucherOfCustomerByUserIdQuery(
    string? filterStatus,
    string? filterType,
    string? searchTerm,
    string? sortColumn,
    string? sortOrder,
    int? page,
    int? pageSize,
    string token) : IQuery<PagedList<CustomerVoucherResponse>>;
