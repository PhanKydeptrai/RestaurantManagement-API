using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void DeleteEmployee(Employee employee)
        {
            _context.Employees.Remove(employee);
        }

        public async Task<bool> GetEmployeeByEmail(string email)
        {
            return await _context.Users
                .Include(e => e.Employee)
                .AnyAsync(e => e.Email == email && e.Employee != null); // Kiểm tra email trong employee đã tồn tại trong user hay chưa
        }
        public  async Task<bool> GetEmployeeByName(string firstname)
        {
            return await _context.Users
                .Include(e => e.Employee)
                .AnyAsync(e => e.FirstName == firstname);
        }
        public async Task<bool> GetEmployeeByPhone(string phone)
        {
            return await _context.Users
                .Include(e => e.Employee)
                .AnyAsync(e => e.PhoneNumber == phone);
        }
        public async Task<Employee?> GetEmployeeById(Guid id)
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
    }
}
