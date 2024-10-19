using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Features.CustomerFeature.DTOs;
using RestaurantManagement.Application.Features.Paging;

namespace RestaurantManagement.Application.Features.CustomerFeature.Queries.CustomerFilter;

public record CustomerFilterQuery(
    string? filter,
    string? searchTerm,
    string? sortColumn,
    string? sortOrder,
    int? page,
    int? pageSize) : IQuery<PagedList<CustomerResponse>>;

