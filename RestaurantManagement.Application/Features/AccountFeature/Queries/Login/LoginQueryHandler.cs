using Microsoft.EntityFrameworkCore;
using NETCore.Encrypt;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.DTOs.CustomerDto;
using RestaurantManagement.Domain.DTOs.LoginDto;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Queries.Login;

public class LoginQueryHandler(
    IApplicationDbContext context, 
    IJwtProvider jwtProvider) : ICommandHandler<LoginQuery, LoginResponse>
{
    public async Task<Result<LoginResponse>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        //Validate request
        var validator = new LoginQueryValidator();
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result<LoginResponse>.Failure(errors!);
        }

        var encryptedPassword = EncryptProvider.Sha256(request.passWord);

        //TODO: REFACTOR
        CustomerLoginResponse? loginResponse = await context.Customers
            .Where(a => a.User.Password == encryptedPassword
            && (a.User.Email == request.loginString
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
            return Result<LoginResponse>.Failure(new[] { new Error("Login Fail", "Invalid login") });
        }

        if (loginResponse.UserStatus != "Activated") //Chỉ khách hàng đã kích hoạt mới được đăng nhập
        {
            return Result<LoginResponse>.Failure(new[] { new Error("Login Fail", "Account is not activated") });
        }

        var token = jwtProvider.GenerateJwtToken(
                loginResponse.UserId,
                loginResponse.Email,
                loginResponse.CustomerType);

        return Result<LoginResponse>.Success(new LoginResponse(token, loginResponse.CustomerType));
    }
}
