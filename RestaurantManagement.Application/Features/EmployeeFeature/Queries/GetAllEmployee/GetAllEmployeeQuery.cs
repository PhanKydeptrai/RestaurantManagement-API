using MediatR;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Queries.GetAllEmployee
{
    public record GetAllEmployeeQuery() : IRequest<Result<List<Employee>>>;

}
