using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RestaurantManagement.Domain.DTOs.CustomerDto;
using RestaurantManagement.Domain.DTOs.EmployeeDto;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Infrastructure.Authentication;

public class JwtProvider : IJwtProvider
{
    SymmetricSecurityKey _key;
    IConfiguration _config;
    public JwtProvider(IConfiguration config)
    {
        _config = config;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
    }
    
    public string GenerateJwtTokenForCustomer(CustomerLoginResponse loginResponse)
    {
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, loginResponse.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, loginResponse.Email),
            new Claim(ClaimTypes.Role, loginResponse.CustomerType)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(10),
            SigningCredentials = credentials,
            Issuer = _config["JWT:Issuer"],
            Audience = _config["JWT:Audience"]

        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
    
    //Generate token for employee
    public string GenerateJwtTokenForEmployee(EmployeeLoginResponse loginResponse)
    {
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, loginResponse.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, loginResponse.Email),
            new Claim(ClaimTypes.Role, loginResponse.Role)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(10),
            SigningCredentials = credentials,
            Issuer = _config["JWT:Issuer"],
            Audience = _config["JWT:Audience"]

        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
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
            Issuer = _config["JWT:Issuer"],
            Audience = _config["JWT:Audience"]

        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
