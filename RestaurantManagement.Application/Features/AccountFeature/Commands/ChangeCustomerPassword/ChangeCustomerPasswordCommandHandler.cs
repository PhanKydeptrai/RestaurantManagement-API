using FluentEmail.Core;
using NETCore.Encrypt;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ChangeCustomerPassword;

public class ChangeCustomerPasswordCommandHandler : ICommandHandler<ChangeCustomerPasswordCommand>
{
    private readonly IUserRepository _userRepository;
    
    private readonly IFluentEmail _fluentEmail;
    private readonly IEmailVerificationTokenRepository _emailVerificationTokenRepository;
    private readonly IEmailVerify _emailVerify;
    private readonly IUnitOfWork _unitOfWork;
    public ChangeCustomerPasswordCommandHandler(
        
        IUserRepository userRepository,
        IFluentEmail fluentEmail,
        IEmailVerificationTokenRepository emailVerificationTokenRepository,
        IEmailVerify emailVerify,
        IUnitOfWork unitOfWork)
    {

        
        _userRepository = userRepository;
        _fluentEmail = fluentEmail;
        _emailVerificationTokenRepository = emailVerificationTokenRepository;
        _emailVerify = emailVerify;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ChangeCustomerPasswordCommand request, CancellationToken cancellationToken)
    {
        var validator = new ChangeCustomerPasswordCommandValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            Error[] errors = validationResult.Errors
                .Select(x => new Error(x.PropertyName, x.ErrorMessage))
                .ToArray();

            return Result.Failure(errors);
        }

        //Lấy token
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Kiểm tra mật khẩu cũ
        var user = await _userRepository.GetUserById(Ulid.Parse(userId));
        string encryptPass = EncryptProvider.Sha256(request.oldPass);
        if (encryptPass != user.Password)
        {
            Error[] errors = { new Error("OldPassword", "Mật khẩu cũ không đúng") };
            return Result.Failure(errors);
        }
        //Tạo token xác thực 
        var token = new EmailVerificationToken
        {
            CreatedDate = DateTime.Now,
            ExpiredDate = DateTime.Now.AddMinutes(5),
            EmailVerificationTokenId = Ulid.NewUlid(),
            UserId = Ulid.Parse(userId),
            Temporary = EncryptProvider.Sha256(request.newPass)
        };
        await _emailVerificationTokenRepository.CreateVerificationToken(token);

        string verificationLink = _emailVerify.CreateLinkForChangePass(token);
        //Gửi mail xác thực 
        await _fluentEmail.To(user.Email).Subject("Xác nhận đổi mật khẩu")
            .Body($"Vui lòng xác nhận để thay đôi mật khẩu bằng cách click vào link sau: <a href='{verificationLink}'>Click me</a>", isHtml: true)
            .SendAsync();

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
