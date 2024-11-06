using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.VerifyChangeCustomerPassword;

public class VerifyChangeCustomerPasswordCommandHandler(
    IEmailVerificationTokenRepository emailVerificationTokenRepository,
    IApplicationDbContext context,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository) : ICommandHandler<VerifyChangeCustomerPasswordCommand>
{
    public async Task<Result> Handle(VerifyChangeCustomerPasswordCommand request, CancellationToken cancellationToken)
    {
        //Lấy token
        var token = await emailVerificationTokenRepository.GetVerificationTokenById(request.tokenId);

        //kiểm tra token

        if (token == null) //token không tồn tại
        {
            Error[] errors = { new Error("Link", "Đường dẫn không hợp lệ") };
            return Result.Failure(errors);
        }

        if (token.ExpiredDate < DateTime.UtcNow) //token hết hạn
        {
            Error[] errors = { new Error("Link", "Đường dẫn đã hết hạn") };
            emailVerificationTokenRepository.RemoveVerificationToken(token);
            await unitOfWork.SaveChangesAsync();
            return Result.Failure(errors);
        }

        //token hợp lệ
        //Cập nhật mật khẩu mới
        var user = await userRepository.GetUserById(token.UserId);
        user.Password = token.Temporary;

        emailVerificationTokenRepository.RemoveVerificationToken(token);
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
