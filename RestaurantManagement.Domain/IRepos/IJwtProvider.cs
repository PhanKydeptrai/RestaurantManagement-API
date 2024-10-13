using RestaurantManagement.Domain.DTOs.CustomerDto;
using RestaurantManagement.Domain.DTOs.EmployeeDto;

namespace RestaurantManagement.Domain.IRepos;

public interface IJwtProvider
{
    //Generate token for customer
    string GenerateJwtTokenForCustomer(CustomerLoginResponse loginResponse);
    //Generate token for employee
    string GenerateJwtTokenForEmployee(EmployeeLoginResponse loginResponse);
    string GenerateJwtToken(string userId, string email, string role);
}
