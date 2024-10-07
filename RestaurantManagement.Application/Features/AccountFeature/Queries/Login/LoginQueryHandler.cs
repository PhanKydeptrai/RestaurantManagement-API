using MediatR;
using Microsoft.EntityFrameworkCore;
using NETCore.Encrypt;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.Entities;
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

    //TODO: Refactor this method
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
        //Lưu ý
        User? user = await _context.Customers
            .Where(a => a.User.Password == encryptedPassword 
            &&(a.User.Email == request.loginString || a.User.Phone == request.loginString) 
            && a.CustomerType == "Subscriber"
            && a.CustomerStatus == "Active")
            .Select(a => a.User).FirstOrDefaultAsync(); 

        
        

        if (user == null)
        {
            Error[] a = { new Error("Login", "Invalid login") };
            return Result<string>.Failure(a);
        }

        if(user.Status != "Activated") //Chỉ khách hàng đã kích hoạt mới được đăng nhập
        {
            Error[] a = { new Error("Login", "Account is not activated") };
            return Result<string>.Failure(a);
        }

        var token = _jwtProvider.GenerateJwtToken(user);
        
        return Result<string>.Success(token);
    }
}
