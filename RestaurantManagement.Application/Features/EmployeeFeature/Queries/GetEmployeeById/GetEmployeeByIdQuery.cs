using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.EmployeeDto;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Queries.GetEmployeeById;

public record GetEmployeeByIdQuery(Ulid id) : IQuery<EmployeeResponse>;
