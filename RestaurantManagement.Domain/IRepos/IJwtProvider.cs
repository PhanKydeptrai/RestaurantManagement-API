using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface IJwtProvider
{
    string GenerateJwtToken(User user);
}
