using MediatR;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Queries.GetAllEmployee
{
    public record GetAllEmployeeQuery() : IRequest<Result<IEnumerable<Employee>>>;

}
