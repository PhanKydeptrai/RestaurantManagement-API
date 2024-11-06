using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos
{
    public class UserRepository(RestaurantManagementDbContext context) : IUserRepository
    {
        public async Task CreateUser(User user)
        {
            await context.Users.AddAsync(user);
        }

        public void DeleteUser(User user)
        {
            context.Users.Remove(user);
        }

        public Task<User> GetUserByEmail(string email)
        {
            return context.Users.FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<User> GetUserById(Ulid id)
        {
            return await context.Users.FindAsync(id);
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await context.Users.ToListAsync();
        }

        public async Task<bool> IsEmailExists(string email)
        {
            return await context.Users.AnyAsync(e => e.Email == email);
        }

        

        public void UpdateUser(User user)
        {
            context.Users.Update(user);
        }
    }
}
