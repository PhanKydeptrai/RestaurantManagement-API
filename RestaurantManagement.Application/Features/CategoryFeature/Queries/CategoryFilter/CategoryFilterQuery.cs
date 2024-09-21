using MediatR;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.CategoryDto;

namespace RestaurantManagement.Application.Features.CategoryFeature.Queries.CategoryFilter;

public record CategoryFilterQuery(string? searchTerm, int page, int pageSize) : IRequest<PagedList<CategoryResponse>>;

