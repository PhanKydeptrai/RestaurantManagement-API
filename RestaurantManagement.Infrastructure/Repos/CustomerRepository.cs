using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Features.CustomerFeature.DTOs;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;
using System.Numerics;

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

    public async Task<CustomerResponse?> GetCustomerById(Guid id)
    {
        return await _context.Customers
            .Include(a => a.User)
            .Where(a => a.CustomerId == id)
            .Select(a => new CustomerResponse
            {
                CustomerId = a.CustomerId,
                FirstName = a.User.FirstName,
                LastName = a.User.LastName,
                Email = a.User.Email,
                PhoneNumber = a.User.PhoneNumber,
                Gender = a.User.Gender,
                UserImage = a.User.UserImage
            }).FirstOrDefaultAsync();
            
    }

    public IQueryable<Customer> GetCustomersQueryable()
    {
        return _context.Customers.AsQueryable();
    }

    public async Task<bool> IsCustomerEmailExist(string email)
    {
        return await _context.Customers
            .Include(c => c.User)
            .AnyAsync(a => a.User.Email == email);
    }

    
    public async Task<bool> IsCustomerEmailExist_update(Guid id,string email)
    {
        return await _context.Customers
            .Include(c => c.User)
            .AnyAsync(a => a.User.Email == email && a.CustomerId != id);
    }

    public async Task<bool> IsCustomerPhoneExist(string phone)
    {
        return await _context.Customers
            .Include(c => c.User)
            .AnyAsync(a => a.User.PhoneNumber == phone);
    }

    public async Task<bool> IsCustomerPhoneExist_update(Guid id, string phone)
    {
        return await _context.Customers
            .Include(c => c.User)
            .AnyAsync(a => a.User != null && a.User.PhoneNumber == phone && a.CustomerId != id);
    }
    
    public void UpdateCustomer(Customer customer)
    {
        _context.Customers.Update(customer);
    }
}
