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
        return await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
    }

    public IQueryable<Customer> GetCustomersQueryable()
    {
        IQueryable<Customer> customers = _context.Customers;
        return customers;
    }

    public async Task<bool> IsCustomerEmailExist(string email)
    {
        //Yêu cầu: Kiểm tra xem email đã tồn tại trong database chưa
        //Nếu có thì trả về true, ngược lại trả về false
        
        User[] userArray = await _context.Users
                                    .Where(u => u.Email == email)
                                    .ToArrayAsync();

        foreach(var user in userArray)
        {
            if(await _context.Customers.AnyAsync(u => u.UserId == user.UserId))
            {
                Console.WriteLine(await _context.Customers.AnyAsync(u => u.UserId == user.UserId));
                return true;
                
            }
        }
        return false;
    }

    public async Task<bool> IsCustomerPhoneExist(string phone)
    {
        User[] userArray = await _context.Users
                                    .Where(u => u.PhoneNumber == phone)
                                    .ToArrayAsync();

        foreach(var user in userArray)
        {
            if(await _context.Customers.AnyAsync(u => u.UserId == user.UserId))
            {
                return true;
            }
        }
        return false;
    }

    public void UpdateCustomer(Customer customer)
    {
        _context.Customers.Update(customer);
    }
}
