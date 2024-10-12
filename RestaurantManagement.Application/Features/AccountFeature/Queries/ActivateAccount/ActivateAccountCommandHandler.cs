using FluentEmail.Core;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Queries.ActivateAccount;

public class ActivateAccountCommandHandler : ICommandHandler<ActivateAccountCommand>
{
    private readonly IEmailVerificationTokenRepository _emailVerificationTokenRepository;
    private readonly IApplicationDbContext _context;
    private readonly IEmailVerify _emailVerify;
    private readonly IFluentEmail _fluentEmail;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateAccountCommandHandler(
        IEmailVerificationTokenRepository emailVerificationTokenRepository, 
        IApplicationDbContext context, 
        IEmailVerify emailVerify, 
        IFluentEmail fluentEmail, 
        IUnitOfWork unitOfWork)
    {
        _emailVerificationTokenRepository = emailVerificationTokenRepository;
        _context = context;
        _emailVerify = emailVerify;
        _fluentEmail = fluentEmail;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ActivateAccountCommand request, CancellationToken cancellationToken)
    {
        EmailVerificationToken token = await _emailVerificationTokenRepository.GetVerificationTokenById(request.tokenId);
        if (token is null || token.User.Status == "Activated")
        {
            Error[] errors = new[]
            {
                new Error("EmailVerificationToken", "Token is invalid")
            };
            return Result.Failure(errors);
        }

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
            string? verificationLink = _emailVerify.Create(emailVerificationToken);
            //gui mail
            await _fluentEmail.To(token.User.Email).Subject("Kích hoạt tài khoản")
            .Body($"Vui lòng kích hoạt tài khoản bằng cách click vào link sau: <a href='{verificationLink}'>Click me</a>", isHtml: true)
            .SendAsync();

            await _context.EmailVerificationTokens.AddAsync(emailVerificationToken);
            _emailVerificationTokenRepository.RemoveVerificationToken(token);
            await _unitOfWork.SaveChangesAsync();
            return Result.Failure(errors);
        }

        token.User.Status = "Activated";
        _emailVerificationTokenRepository.RemoveVerificationToken(token);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
