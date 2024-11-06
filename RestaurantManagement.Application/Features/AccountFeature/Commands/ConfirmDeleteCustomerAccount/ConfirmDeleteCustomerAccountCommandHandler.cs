using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ConfirmDeleteCustomerAccount;

public class ConfirmDeleteCustomerAccountCommandHandler(
    IUnitOfWork unitOfWork,
    ICustomerRepository customerAccountRepository,
    IEmailVerificationTokenRepository emailVerificationTokenRepository) : ICommandHandler<ConfirmDeleteCustomerAccountCommand>
{
    public async Task<Result> Handle(ConfirmDeleteCustomerAccountCommand request, CancellationToken cancellationToken)
    {
        EmailVerificationToken token = await emailVerificationTokenRepository.GetVerificationTokenById(request.tokenId);
        //check trạng thái tài khoản
        if (token is null || token.User.Status == "Deleted")
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
            return Result.Failure(errors);
        }

        //Delete customer
        await customerAccountRepository.DeleteCustomer(token.UserId);
        emailVerificationTokenRepository.RemoveVerificationToken(token);
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
