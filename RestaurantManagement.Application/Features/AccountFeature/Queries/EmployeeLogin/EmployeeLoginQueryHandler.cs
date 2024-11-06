using Microsoft.EntityFrameworkCore;
using NETCore.Encrypt;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.DTOs.EmployeeDto;
using RestaurantManagement.Domain.DTOs.LoginDto;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Queries.EmployeeLogin;

public class EmployeeLoginQueryHandler : IQueryHandler<EmployeeLoginQuery, LoginResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IApplicationDbContext _context;

    public EmployeeLoginQueryHandler(
        IApplicationDbContext context,
        IJwtProvider jwtProvider,
        IEmployeeRepository employeeRepository)
    {
        _context = context;
        _jwtProvider = jwtProvider;
        _employeeRepository = employeeRepository;
    }

    public async Task<Result<LoginResponse>> Handle(EmployeeLoginQuery request, CancellationToken cancellationToken)
    {
       
        var validator = new EmployeeLoginQueryValidator();
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result<LoginResponse>.Failure(errors);
        }
        //encrypt the password
        var encryptedPassword = EncryptProvider.Sha256(request.passWord);

        EmployeeLoginResponse? loginResponse = await _context.Employees
            .Where(a => a.User.Password == encryptedPassword
                        && (a.User.Email == request.loginString
                            || a.User.Phone == request.loginString))
            .Select(a => new EmployeeLoginResponse(
                a.User.UserId.ToString(),
                a.User.Email,
                a.EmployeeStatus,
                a.Role))
            .FirstOrDefaultAsync();


        if (loginResponse == null || loginResponse.EmployeeStatus != "Active")
        {
            Error[] error = { new Error("Login Fail", "Invalid login") };
            return Result<LoginResponse>.Failure(error);
        }

        string token = _jwtProvider.GenerateJwtToken(
            loginResponse.UserId,
            loginResponse.Email,
            loginResponse.Role);

        return Result<LoginResponse>.Success(new LoginResponse(token, loginResponse.Role));
    }
}
