using MediatR;
using RestaurantManagement.Application.Features.CustomerFeature.DTOs;
using RestaurantManagement.Application.Features.Paging;

namespace RestaurantManagement.Application.Features.CustomerFeature.Queries.CustomerFilter;

public record CustomerFilterQuery(
    string? searchTerm, 
    int page, 
    int pageSize) : IRequest<PagedList<CustomerResponse>>;

