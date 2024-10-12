using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetUsers();
    Task<User> GetUserById(Ulid id);
    Task<User> GetUserByEmail(string email);
    Task<bool> IsEmailExists(string email);
    Task CreateUser(User user);
    void UpdateUser(User user);
    void DeleteUser(User user);

}
