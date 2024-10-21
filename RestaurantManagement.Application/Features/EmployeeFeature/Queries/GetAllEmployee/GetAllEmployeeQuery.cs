using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.EmployeeDto;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Queries.GetAllEmployee;

public record GetAllEmployeeQuery(
    string? filterGender,
    string? filterRole,
    string? filterStatus,
    string? searchTerm,
    string? sortColumn,
    string? sortOrder,
    int? page,
    int? pageSize) : IQuery<PagedList<EmployeeResponse>>;
