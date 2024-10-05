using MediatR;
using Microsoft.EntityFrameworkCore;
using NETCore.Encrypt;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.DTOs.Common;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;

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

    //TODO: Refactor this method
    public async Task<Result<string>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var result = new Result<string>();
        //validate
        var loginQueryValidator = new LoginQueryValidator();
        var validationResult = loginQueryValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            result.Errors = validationResult.Errors
                .Select(x => x.ErrorMessage).ToArray();
            return result;
        }

        var encryptedPassword = EncryptProvider.Sha256(request.passWord);

        User? user = await _context.Customers
            .Where(a => a.User.Password == encryptedPassword 
            &&(a.User.Email == request.loginString || a.User.Phone == request.loginString) 
            &&a.CustomerType == "Subscriber")
            .Select(a => a.User).FirstOrDefaultAsync(); 

        if (user == null)
        {
            result.Errors = new string[] { "Invalid login credentials" };
            return result;
        }

        if(user.Status != "Activated") //Chỉ khách hàng đã kích hoạt mới được đăng nhập
        {
            result.Errors = new string[] { "Please active your account" };
            return result;
        }

        result.ResultValue = _jwtProvider.GenerateJwtToken(user);
        result.IsSuccess = true;
        return result;
    }
}
