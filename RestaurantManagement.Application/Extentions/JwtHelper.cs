using System.IdentityModel.Tokens.Jwt;

namespace RestaurantManagement.Application.Extentions;

public static class JwtHelper
{
    public static IDictionary<string, string> DecodeJwt(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken = handler.ReadJwtToken(token);

        var claims = jwtToken.Claims.ToDictionary(c => c.Type, c => c.Value);

        return claims;
    }
}
