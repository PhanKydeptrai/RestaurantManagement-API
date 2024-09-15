using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class CustomerRepository : ICustomerRepository
{
    private readonly RestaurantManagementDbContext _context;
    public CustomerRepository(RestaurantManagementDbContext context)
    {
        _context = context;
    }
    public async Task CreateCustomer(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
    }

    public void DeleteMeal(Customer customer)
    {
        _context.Customers.Remove(customer);
    }

    public async Task<IEnumerable<Customer>> GetAllCustomers()
    {
        return await _context.Customers.ToListAsync();
    }

    public async Task<Customer?> GetCustomerById(Guid id)
    {
        return await _context.Customers.FindAsync(id);
    }

    public IQueryable<Customer> GetCustomersQueryable()
    {
        return _context.Customers.AsQueryable();
    }

    public async Task<bool> IsCustomerEmailExist(string email)
    {
        return await _context.Customers
            .Include(c => c.User)
            .AnyAsync(a => a.User != null && a.User.Email == email);

    }

    public async Task<bool> IsCustomerPhoneExist(string phone)
    {
        return await _context.Customers
            .Include(c => c.User)
            .AnyAsync(a => a.User != null && a.User.PhoneNumber == phone);
    }

    public void UpdateCustomer(Customer customer)
    {
        _context.Customers.Update(customer);
    }
}
