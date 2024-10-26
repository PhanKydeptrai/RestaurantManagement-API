using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly RestaurantManagementDbContext _context;
        public EmployeeRepository(RestaurantManagementDbContext context)
        {
            _context = context;
        }
        public async Task CreateEmployee(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
        }

        public async Task DeleteEmployee(Ulid userId)
        {
            await _context.Employees
                .Where(a => a.UserId == userId)
                .ExecuteUpdateAsync(a =>
                a.SetProperty(a => a.EmployeeStatus, "Deleted"));

            await _context.Users
                .Where(a => a.UserId == userId)
                .ExecuteUpdateAsync(a =>
                a.SetProperty(a => a.Status, "Deleted"));
        }

        public async Task<bool> IsEmployyeEmailExist(string email)
        {
            return await _context.Employees
                .Include(e => e.User)
                .AnyAsync(e => e.User.Email == email); // Kiểm tra email trong employee đã tồn tại trong user hay chưa
        }

        //Kiểm tra tình trạng hoạt động của tài khoản
        public async Task<bool> IsEmployeeAccountActive(string email)
        {
            return await _context.Employees.Include(e => e.User)
                .AnyAsync(e => e.User.Email == email && e.EmployeeStatus == "Active");
        }

        public async Task<bool> IsEmployeePhoneExist(string phone)
        {
            return await _context.Employees
                .Include(e => e.User)
                .AnyAsync(e => e.User.Phone == phone);
        }

        public async Task<bool> IsEmployeePhoneExist_update(Ulid id, string phone)
        {
            return await _context.Employees
                .Select(a => a.User)
                .AnyAsync(a => a.Phone == phone && a.Employee.EmployeeId != id);
        }

        public async Task<bool> IsEmployeeEmailExist_update(Ulid id, string email)
        {
            return await _context.Employees
                .Select(a => a.User)
                .AnyAsync(a => a.Email == email && a.Employee.EmployeeId != id);
        }

        public async Task<Employee?> GetEmployeeById(Ulid id)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == id);
        }
        public async Task<IEnumerable<Employee>> GetEmployees()
        {
            return await _context.Employees.ToListAsync();

        }

        public void UpdateEmployee(Employee employee)
        {
            _context.Employees.Update(employee);
        }

        public IQueryable<Employee> GetEmployeeQueryable(string firstname)
        {
            return _context.Employees.AsQueryable();
        }

        public async Task<string?> GetEmployeeRole(Ulid id)
        {
            return await _context.Employees
                .Where(e => e.UserId == id)
                .Select(e => e.Role)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsEmployeeExist(Ulid id)
        {
            return await _context.Employees.AnyAsync(e => e.UserId == id && e.EmployeeStatus == "Active");
        }
    }
}
