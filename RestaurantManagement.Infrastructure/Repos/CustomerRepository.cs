using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Features.CustomerFeature.DTOs;
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

    public void DeleteCustomer(Customer customer)
    {
        _context.Customers.Remove(customer);
    }

    public async Task<IEnumerable<Customer>> GetAllCustomers()
    {
        return await _context.Customers.ToListAsync();
    }

    public async Task<CustomerResponse?> GetCustomerById(Ulid id)
    {
        return await _context.Customers
            .Where(a => a.UserId == id)
            .Select(a => new CustomerResponse(
                a.CustomerId,
                a.User.FirstName,
                a.User.LastName,
                a.User.Email,
                a.User.Phone,
                a.User.Gender,
                a.User.Status,
                a.CustomerStatus,
                a.CustomerType,
                a.User.ImageUrl
            )).FirstOrDefaultAsync();
    }

    public async Task<bool> IsCustomerExist(Ulid id)
    {
        return await _context.Customers
            .AsNoTracking()
            .AnyAsync(a => a.UserId == id);
    }

    public IQueryable<Customer> GetCustomersQueryable()
    {
        return _context.Customers.AsQueryable();
    }

    public async Task<bool> IsCustomerEmailExist(string email)
    {
        return await _context.Customers
            .Include(c => c.User)
            .AnyAsync(a => a.User.Email == email && a.CustomerType == "Subscriber");
    }

    public async Task<bool> IsCustomerAccountActive(string email)
    {
        return await _context.Customers
            .Include(c => c.User)
            .AnyAsync(a => a.User.Email == email && a.CustomerType == "Subscriber" && a.CustomerStatus == "Active");
    }

    public async Task<bool> IsCustomerPhoneExist(string phone)
    {
        return await _context.Customers
            .Include(c => c.User)
            .AnyAsync(a => a.User.Phone == phone && a.CustomerType == "Subscriber");
    }

    public async Task<bool> IsCustomerEmailExist_update(Ulid id, string email)
    {
        return await _context.Customers
            .Include(c => c.User)
            .AnyAsync(a => a.User.Email == email && a.UserId != id);
    }

    public async Task<bool> IsCustomerPhoneExist_update(Ulid id, string phone)
    {
        return await _context.Customers
            .Include(c => c.User)
            .AnyAsync(a => a.User != null && a.User.Phone == phone && a.UserId != id);
    }

    public void UpdateCustomer(Customer customer)
    {
        _context.Customers.Update(customer);
    }


    public async Task DeleteCustomer(Ulid id)
    {
        // await _context.Customers.Include(c => c.User)
        //     .Where(c => c.UserId == id)
        //     .ExecuteUpdateAsync(a => a
        //     .SetProperty(a => a.CustomerStatus, "Deleted")
        //     .SetProperty(a => a.User.Status, "Deleted"));
        
        // Cập nhật trạng thái của Customer
        await _context.Customers
            .Where(c => c.UserId == id)
            .ExecuteUpdateAsync(a => a.SetProperty(a => a.CustomerStatus, "Deleted"));

        // Cập nhật trạng thái của User
        await _context.Users
            .Where(u => u.UserId == id)
            .ExecuteUpdateAsync(a => a.SetProperty(a => a.Status, "Deleted"));
    }
}
