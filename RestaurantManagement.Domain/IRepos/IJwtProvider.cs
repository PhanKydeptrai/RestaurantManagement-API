using Microsoft.AspNetCore.Http;

namespace RestaurantManagement.Domain.IRepos;

public interface IJwtProvider
{
    string GenerateJwtToken(string userId, string email, string role);
    string GetTokenFromHeader(HttpContext header);
}
