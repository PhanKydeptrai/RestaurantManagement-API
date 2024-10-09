using MediatR;
using Microsoft.EntityFrameworkCore;
using NETCore.Encrypt;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.DTOs.CustomerDto;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Queries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, Result<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtProvider _jwtProvider;
    public LoginQueryHandler(IApplicationDbContext context, IJwtProvider jwtProvider)
    {
        _context = context;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<string>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        //validate
        var loginQueryValidator = new LoginQueryValidator();
        var validationResult = loginQueryValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(x => new Error(x.ErrorCode, x.ErrorMessage))
                .ToArray();
            return Result<string>.Failure(errors);
        }

        var encryptedPassword = EncryptProvider.Sha256(request.passWord);
        //REFACTOR
        CustomerLoginResponse? loginResponse = await _context.Customers
            .Where(a => a.User.Password == encryptedPassword 
            &&(a.User.Email == request.loginString 
            || a.User.Phone == request.loginString) 
            && a.CustomerType == "Subscriber"
            && a.CustomerStatus == "Active")
            .Select(a => new CustomerLoginResponse(
                a.User.UserId.ToString(), 
                a.User.Email, 
                a.CustomerType,
                a.User.Status))
            .FirstOrDefaultAsync(); 

    
        if (loginResponse == null)
        {
            Error[] a = { new Error("Login Fail", "Invalid login") };
            return Result<string>.Failure(a);
        }

        if(loginResponse.UserStatus != "Activated") //Chỉ khách hàng đã kích hoạt mới được đăng nhập
        {
            Error[] a = { new Error("Login Fail", "Account is not activated") };
            return Result<string>.Failure(a);
        }

        var token = _jwtProvider.GenerateJwtTokenForCustomer(loginResponse);

        return Result<string>.Success(token);
    }
}
