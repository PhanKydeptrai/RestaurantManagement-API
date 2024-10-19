using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.MealDto;

namespace RestaurantManagement.Application.Features.MealFeature.Queries.GetAllMeal;

public record GetAllMealQuery(string? searchTerm,
    string? sortColumn,
    string? sortOrder,
    int page,
    int pageSize) : IQuery<PagedList<MealResponse>>;
