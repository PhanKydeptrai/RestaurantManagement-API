using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos
{
    public class UserRepository : IUserRepository
    {
        private readonly RestaurantManagementDbContext _context;
        public UserRepository(RestaurantManagementDbContext context)
        {
            _context = context;
        }
        public async Task CreateUser(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public void DeleteUser(User user)
        {
            _context.Users.Remove(user);
        }

        public Task<User> GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefaultAsync(e => e.Email == email);   
        }

        public async Task<User> GetUserById(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<bool> IsEmailExists(string email)
        {
            return await _context.Users.AnyAsync(e => e.Email == email);
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
        }
    }
}
