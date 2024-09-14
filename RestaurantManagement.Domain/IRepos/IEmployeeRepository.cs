using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetEmployees();
    Task<Employee?> GetEmployeeById(Guid id);
    Task<bool> GetEmployeeByEmail(string email);
    Task<bool> GetEmployeeByName(string firstname);
    Task<bool> GetEmployeeByPhone(string phone);
    Task CreateEmployee(Employee employee);
    void UpdateEmployee(Employee employee);
    void DeleteEmployee(Employee employee);
    
}
