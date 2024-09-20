using MediatR;
using RestaurantManagement.Application.Features.CategoryFeature.DTOs;

namespace RestaurantManagement.Application.Features.CategoryFeature.Queries.CategoryFilter;

public record CategoryFilterQuery(string? searchTerm, int page, int pageSize) : IRequest<PagedList<CategoryResponse>>;

