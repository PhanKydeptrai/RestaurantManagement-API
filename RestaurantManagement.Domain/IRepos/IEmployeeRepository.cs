using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetEmployees();
    Task<Employee?> GetEmployeeById(Guid id);
    Task<bool> IsEmployyeEmailExist(string email);
    IQueryable<Employee> GetEmployeeQueryable(string firstname);
    Task<bool> IsEmployeePhoneExist(string phone);
    Task CreateEmployee(Employee employee);
    void UpdateEmployee(Employee employee);
    void DeleteEmployee(Employee employee);
    
}
