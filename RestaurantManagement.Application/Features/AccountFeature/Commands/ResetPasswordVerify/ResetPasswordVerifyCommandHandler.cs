using FluentEmail.Core;
using NETCore.Encrypt;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ResetPasswordVerify;

public class ResetPasswordVerifyCommandHandler : ICommandHandler<ResetPasswordVerifyCommand>
{
    private readonly IEmailVerificationTokenRepository _emailVerificationTokenRepository;
    private readonly IEmailVerify _emailVerify;
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFluentEmail _fluentEmail;
    private readonly IUserRepository _userRepository;
    public ResetPasswordVerifyCommandHandler(
        IEmailVerificationTokenRepository emailVerificationTokenRepository,
        IEmailVerify emailVerify,
        IApplicationDbContext applicationDbContext,
        IUnitOfWork unitOfWork,
        IApplicationDbContext context,
        IFluentEmail fluentEmail,
        IUserRepository userRepository)
    {
        _emailVerificationTokenRepository = emailVerificationTokenRepository;
        _emailVerify = emailVerify;
        _applicationDbContext = applicationDbContext;
        _unitOfWork = unitOfWork;
        _fluentEmail = fluentEmail;
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(ResetPasswordVerifyCommand request, CancellationToken cancellationToken)
    {
        EmailVerificationToken token = await _emailVerificationTokenRepository.GetVerificationTokenById(request.tokenId);
        if (token is null) //kiểm tra token có tồn tại không
        {
            Error[] errors = new[]
            {
                new Error("EmailVerificationToken", "Link is invalid")
            };
            return Result.Failure(errors);
        }

        if (token.ExpiredDate < DateTime.UtcNow) //kiểm tra thời gian hết hạn của token
        {
            Error[] errors = new[]
            {
                new Error("EmailVerificationToken", "Link is invalid")
            };
            return Result.Failure(errors);
        }
        //Tạo một mật khẩu mới
        string randomPass = RandomStringGenerator.GenerateRandomString();
        var user = await _userRepository.GetUserById(token.UserId);
        user.Password = EncryptProvider.Sha256(randomPass);

        await _fluentEmail.To(token.User.Email).Subject("Mật khẩu mới")
            .Body($"Mật khẩu mới của bạn là: {randomPass}", isHtml: true)
            .SendAsync();
        //Xóa token
        _emailVerificationTokenRepository.RemoveVerificationToken(token);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}

