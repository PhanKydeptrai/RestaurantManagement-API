using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface IEmployeeRepository
{
    Task UpdateEmployeeRole(Ulid id, string role);
    Task<string?> GetEmployeeRole(Ulid id);
    Task<IEnumerable<Employee>> GetEmployees();
    Task<Employee?> GetEmployeeById(Ulid id);
    Task<bool> IsEmployyeEmailExist(string email);
    Task<bool> IsEmployeeExist(Ulid id);
    IQueryable<Employee> GetEmployeeQueryable(string firstname);
    Task<bool> IsEmployeePhoneExist(string phone);
    Task CreateEmployee(Employee employee);
    void UpdateEmployee(Employee employee);
    Task RestoreEmployee(Ulid userId);
    Task DeleteEmployee(Ulid userId);
    Task<bool> IsEmployeeAccountActive(string email);
    Task<bool> IsEmployeePhoneExist_update(Ulid id, string phone);
    Task<bool> IsEmployeeEmailExist_update(Ulid id, string email);

}
