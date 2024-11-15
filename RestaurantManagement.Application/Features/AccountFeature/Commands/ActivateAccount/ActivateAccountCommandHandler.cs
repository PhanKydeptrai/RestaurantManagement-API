using FluentEmail.Core;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ActivateAccount;

public class ActivateAccountCommandHandler(
    IEmailVerificationTokenRepository emailVerificationTokenRepository,
    IEmailVerify emailVerify,
    IFluentEmail fluentEmail,
    IUnitOfWork unitOfWork) : ICommandHandler<ActivateAccountCommand>
{
    public async Task<Result> Handle(ActivateAccountCommand request, CancellationToken cancellationToken)
    {
        EmailVerificationToken token = await emailVerificationTokenRepository.GetVerificationTokenById(request.tokenId);
        //check trạng thái tài khoản
        if (token is null || token.User.Status == "Activated")
        {
            Error[] errors = new[]
            {
                new Error("EmailVerificationToken", "Token is invalid")
            };
            return Result.Failure(errors);
        }

        //check hạn token
        if (token.ExpiredDate < DateTime.UtcNow)
        {
            Error[] errors = new[]
            {
                new Error("EmailVerificationToken", "This link is exprired, Please check your email to get new link!")
            };

            var emailVerificationToken = new EmailVerificationToken
            {
                EmailVerificationTokenId = Ulid.NewUlid(),
                ExpiredDate = DateTime.UtcNow.AddDays(1),
                UserId = token.UserId,
                CreatedDate = DateTime.UtcNow
            };

            //TODO: Refactor thành phương thức send mail
            string? verificationLink = emailVerify.Create(emailVerificationToken);
            //TODO: Xử lý lỗi gửi mail
            await fluentEmail.To(token.User.Email).Subject("Nhà hàng Nhum nhum - Thông báo kích hoạt tài khoản")
            .Body($"Vui lòng kích hoạt tài khoản bằng cách click vào link sau: <a href='{verificationLink}'>Click me</a>", isHtml: true)
            .SendAsync();

            await emailVerificationTokenRepository.CreateVerificationToken(emailVerificationToken);
            emailVerificationTokenRepository.RemoveVerificationToken(token);
            await unitOfWork.SaveChangesAsync();
            return Result.Failure(errors);
        }

        token.User.Status = "Activated";
        emailVerificationTokenRepository.RemoveVerificationToken(token);
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
