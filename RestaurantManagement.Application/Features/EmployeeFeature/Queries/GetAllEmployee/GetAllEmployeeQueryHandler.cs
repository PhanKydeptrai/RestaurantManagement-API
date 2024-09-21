using MediatR;
using RestaurantManagement.Domain.DTOs.Common;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Queries.GetAllEmployee
{
    public class GetAllEmployeeQueryHandler : IRequestHandler<GetAllEmployeeQuery, Result<IEnumerable<Employee>>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository;
        public GetAllEmployeeQueryHandler(IEmployeeRepository employeeRepository, IUserRepository userRepository)
        {
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
        }

        public async Task<Result<IEnumerable<Employee>>> Handle(GetAllEmployeeQuery request, CancellationToken cancellationToken)
        {
            Result<IEnumerable<Employee>> result = new Result<IEnumerable<Employee>>
            {
                ResultValue = new List<Employee>(),
                IsSuccess = false
            };
            var employee = await _employeeRepository.GetEmployees();
            if (employee.Any())
            {
                result.ResultValue = employee;
                result.IsSuccess = true;
            }
            return result;
        }
    }
}
