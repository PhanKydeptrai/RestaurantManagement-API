using RestaurantManagement.Domain.Entities;
namespace RestaurantManagement.Domain.IRepos;

public interface ICustomerRepository
{
    //CRUD
    Task CreateCustomer(Customer customer);
    Task<IEnumerable<Customer>> GetAllCustomers();
    Task<Customer?> GetCustomerById(Guid id);
    void UpdateCustomer(Customer customer);
    void DeleteMeal(Customer customer);

    //Queries
    IQueryable<Customer> GetCustomersQueryable();
    Task<bool> IsCustomerEmailExist(string email);
    Task<bool> IsCustomerPhoneExist(string phone);
}
