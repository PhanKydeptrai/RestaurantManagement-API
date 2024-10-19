using Microsoft.EntityFrameworkCore;
using NETCore.Encrypt;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.DTOs.EmployeeDto;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Queries.EmployeeLogin;

public class EmployeeLoginQueryHandler : IQueryHandler<EmployeeLoginQuery, string>
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

    public async Task<Result<string>> Handle(EmployeeLoginQuery request, CancellationToken cancellationToken)
    {
        //validate the request
        var validator = new EmployeeLoginQueryValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            Error[] errors = validationResult.Errors
                .Select(x => new Error(x.ErrorCode, x.ErrorMessage))
                .ToArray();
            return Result<string>.Failure(errors);
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
            Error[] errors = { new Error("Login Fail", "Invalid login") };
            return Result<string>.Failure(errors);
        }
        string token = _jwtProvider.GenerateJwtToken(
            loginResponse.UserId,
            loginResponse.Email,
            loginResponse.Role);

        return Result<string>.Success(token);
    }
}
