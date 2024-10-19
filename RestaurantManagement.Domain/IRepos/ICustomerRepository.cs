using RestaurantManagement.Application.Features.CustomerFeature.DTOs;
using RestaurantManagement.Domain.Entities;
namespace RestaurantManagement.Domain.IRepos;

public interface ICustomerRepository
{
    //CRUD
    Task CreateCustomer(Customer customer);
    Task<IEnumerable<Customer>> GetAllCustomers();
    Task<CustomerResponse?> GetCustomerById(Ulid id);
    void UpdateCustomer(Customer customer);
    void DeleteCustomer(Customer customer);

    //Queries
    IQueryable<Customer> GetCustomersQueryable();
    Task<bool> IsCustomerEmailExist(string email);
    Task<bool> IsCustomerEmailExist_update(Ulid id, string email);
    Task<bool> IsCustomerAccountActive(string email);
    Task<bool> IsCustomerPhoneExist(string phone);

    Task<bool> IsCustomerPhoneExist_update(Ulid id, string phone);
}
