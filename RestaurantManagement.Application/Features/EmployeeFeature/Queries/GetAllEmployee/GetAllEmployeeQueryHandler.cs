using MediatR;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Queries.GetAllEmployee
{
    public class GetAllEmployeeQueryHandler : IRequestHandler<GetAllEmployeeQuery, Result<List<Employee>>>
    {
        // private readonly IEmployeeRepository _employeeRepository;
        // private readonly IUserRepository _userRepository;
        // public GetAllEmployeeQueryHandler(IEmployeeRepository employeeRepository, IUserRepository userRepository)
        // {
        //     _employeeRepository = employeeRepository;
        //     _userRepository = userRepository;
        // }

        // public async Task<Result<IEnumerable<Employee>>> Handle(GetAllEmployeeQuery request, CancellationToken cancellationToken)
        // {

        //     var employee = await _employeeRepository.GetEmployees();
        //     if (employee.Any())
        //     {
        //         result.ResultValue = employee;
        //         result.IsSuccess = true;
        //     }
        //     return result;
        public Task<Result<List<Employee>>> Handle(GetAllEmployeeQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
