using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Features.CustomerFeature.DTOs;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class CustomerRepository(RestaurantManagementDbContext context) : ICustomerRepository
{
    public async Task CreateCustomer(Customer customer)
    {
        await context.Customers.AddAsync(customer);
    }
    public async Task<Ulid> GetUserIdByEmail(string email)
    {
        return await context.Customers
            .Where(a => a.User.Email == email)
            .Select(a => a.UserId)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> IsCustomerExist(string email)
    {
        return await context.Customers
            .Include(c => c.User)
            .AnyAsync(a => a.User.Email == email);
    }

    public void DeleteCustomer(Customer customer)
    {
        context.Customers.Remove(customer);
    }

    public async Task<IEnumerable<Customer>> GetAllCustomers()
    {
        return await context.Customers.ToListAsync();
    }

    public async Task<CustomerResponse?> GetCustomerById(Ulid id)
    {
        return await context.Customers
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
        return await context.Customers
            .AsNoTracking()
            .AnyAsync(a => a.UserId == id);
    }

    public async Task<bool> IsCustomerIdActive(Ulid id)
    {
        return await context.Customers
            .AsNoTracking()
            .AnyAsync(a => a.UserId == id && a.CustomerStatus == "Active" && a.CustomerType == "Subscriber");
    }

    public IQueryable<Customer> GetCustomersQueryable()
    {
        return context.Customers.AsQueryable();
    }

    public async Task<bool> IsCustomerEmailExist(string email)
    {
        return await context.Customers
            .Include(c => c.User)
            .AnyAsync(a => a.User.Email == email && a.CustomerType == "Subscriber");
    }

    public async Task<bool> IsCustomerAccountActive(string email)
    {
        return await context.Customers
            .Include(c => c.User)
            .AnyAsync(a => a.User.Email == email && a.CustomerType == "Subscriber" && a.CustomerStatus == "Active");
    }

    public async Task<bool> IsCustomerHasThisPhoneNumberActive(string phone)
    {
        return await context.Customers
            .AsNoTracking()
            .Include(c => c.User)
            .AnyAsync(a => a.User.Phone == phone && a.CustomerType == "Subscriber" && a.CustomerStatus == "Active");
    }

    public async Task<bool> IsCustomerPhoneExist(string phone) //Kiểm tra xem số điện thoại này có được đăng ký hay chưa
    {
        return await context.Customers
            .AsNoTracking()
            .Include(c => c.User)
            .AnyAsync(a => a.User.Phone == phone && a.CustomerType == "Subscriber");
    }

    public async Task<bool> IsCustomerEmailExist_update(Ulid id, string email)
    {
        return await context.Customers
            .Include(c => c.User)
            .AnyAsync(a => a.User.Email == email && a.UserId != id);
    }

    public async Task<bool> IsCustomerPhoneExist_update(Ulid id, string phone)
    {
        return await context.Customers
            .Include(c => c.User)
            .AnyAsync(a => a.User != null && a.User.Phone == phone && a.UserId != id);
    }

    public void UpdateCustomer(Customer customer)
    {
        context.Customers.Update(customer);
    }


    public async Task DeleteCustomer(Ulid id)
    {
        
        // Cập nhật trạng thái của Customer
        await context.Customers
            .Where(c => c.UserId == id)
            .ExecuteUpdateAsync(a => a.SetProperty(a => a.CustomerStatus, "Deleted"));

        // Cập nhật trạng thái của User
        await context.Users
            .Where(u => u.UserId == id)
            .ExecuteUpdateAsync(a => a.SetProperty(a => a.Status, "Deleted"));
    }
}
