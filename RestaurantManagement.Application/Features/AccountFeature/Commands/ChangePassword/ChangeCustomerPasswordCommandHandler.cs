using FluentEmail.Core;
using NETCore.Encrypt;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ChangeCustomerPassword;

public class ChangeCustomerPasswordCommandHandler(

    IUserRepository userRepository,
    IFluentEmail fluentEmail,
    IEmailVerificationTokenRepository emailVerificationTokenRepository,
    IEmailVerify emailVerify,
    IUnitOfWork unitOfWork) : ICommandHandler<ChangePasswordCommand>
{
    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        //Validate request
        var validator = new ChangeCustomerPasswordCommandValidator();
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        //Lấy token
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Kiểm tra mật khẩu cũ
        var user = await userRepository.GetUserById(Ulid.Parse(userId));
        string encryptPass = EncryptProvider.Sha256(request.oldPass);
        if (encryptPass != user.Password)
        {
            Error[] error = { new Error("OldPassword", "Mật khẩu cũ không đúng") };
            return Result.Failure(error);
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
        await emailVerificationTokenRepository.CreateVerificationToken(token);

        string verificationLink = emailVerify.CreateLinkForChangePass(token);
        //Gửi mail xác thực 
        //TODO: Xử lý lỗi gửi mail
        await fluentEmail.To(user.Email).Subject("Nhà hàng Nhum nhum - Xác nhận thay đổi mật khẩu")
            .Body($"Vui lòng xác nhận để thay đôi mật khẩu bằng cách click vào link sau: <a href='{verificationLink}'>Click me</a>", isHtml: true)
            .SendAsync();

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
