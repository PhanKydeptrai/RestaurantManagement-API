using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RestaurantManagement.Domain.IRepos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestaurantManagement.Infrastructure.Authentication;

public class JwtProvider : IJwtProvider
{
    SymmetricSecurityKey _key;
    IConfiguration _config;
    public JwtProvider(IConfiguration config)
    {
        _config = config;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["SigningKey"]));
    }



    public string GenerateJwtToken(string userId, string email, string role)
    {
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(10),
            SigningCredentials = credentials,
            Issuer = _config["Issuer"],
            Audience = _config["Audience"]

        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string GetTokenFromHeader(HttpContext httpContext)
    {
        //lấy token
        string token = httpContext.Request.Headers["Authorization"]
            .FirstOrDefault()
            .Substring("Bearer ".Length)
            .Trim();

        return token;
    }
}
