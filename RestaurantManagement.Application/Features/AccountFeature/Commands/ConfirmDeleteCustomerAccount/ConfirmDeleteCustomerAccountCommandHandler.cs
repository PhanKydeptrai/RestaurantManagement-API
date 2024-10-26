using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ConfirmDeleteCustomerAccount;

public class ConfirmDeleteCustomerAccountCommandHandler : ICommandHandler<ConfirmDeleteCustomerAccountCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomerRepository _customerAccountRepository;
    private readonly IEmailVerificationTokenRepository _emailVerificationTokenRepository;

    public ConfirmDeleteCustomerAccountCommandHandler(
        IUnitOfWork unitOfWork,
        ICustomerRepository customerAccountRepository,
        IEmailVerificationTokenRepository emailVerificationTokenRepository)
    {
        _unitOfWork = unitOfWork;
        _customerAccountRepository = customerAccountRepository;
        _emailVerificationTokenRepository = emailVerificationTokenRepository;
    }

    public async Task<Result> Handle(ConfirmDeleteCustomerAccountCommand request, CancellationToken cancellationToken)
    {
        EmailVerificationToken token = await _emailVerificationTokenRepository.GetVerificationTokenById(request.tokenId);
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
        await _customerAccountRepository.DeleteCustomer(token.UserId);
        _emailVerificationTokenRepository.RemoveVerificationToken(token);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
