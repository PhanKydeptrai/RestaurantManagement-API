using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ForgotPassword;

internal class ForgotCustomerPasswordCommandHandler : ICommandHandler<ForgotCustomerPasswordCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFluentEmail _fluentEmail;
    private readonly IEmailVerify _emailVerify;
    public ForgotCustomerPasswordCommandHandler(
        ICustomerRepository customerRepository,
        IApplicationDbContext context,
        IUnitOfWork unitOfWork,
        IFluentEmail fluentEmail,
        IEmailVerify emailVerify)
    {
        _customerRepository = customerRepository;
        _context = context;
        _unitOfWork = unitOfWork;
        _fluentEmail = fluentEmail;
        _emailVerify = emailVerify;
    }

    public async Task<Result> Handle(ForgotCustomerPasswordCommand request, CancellationToken cancellationToken)
    {
        var validator = new ForgotCustomerPasswordCommandValidator(_customerRepository);
        var validationResult = await validator.ValidateAsync(request);
        //Validate the request
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(x => new Error(x.ErrorCode, x.ErrorMessage)).ToArray();
            return Result.Failure(errors);
        }


        //Get user by email
        Ulid userId = await _context.Customers
            .Where(a => a.User.Email == request.email)
            .Select(a => a.UserId)
            .FirstOrDefaultAsync();

        //Create email verification token
        var emailVerificationToken = new EmailVerificationToken
        {
            EmailVerificationTokenId = Ulid.NewUlid(),
            ExpiredDate = DateTime.UtcNow.AddMinutes(5),
            UserId = userId,
            CreatedDate = DateTime.UtcNow
        };

        //gửi mail xác thực
        var verificationLink = _emailVerify.CreateLinkForResetPass(emailVerificationToken);
        await _fluentEmail.To(request.email).Subject("Mail xác nhận")
            .Body($"Vui lòng nhấn vào link sau để nhận mật khẩu mới: <a href='{verificationLink}'>Click me</a>" +
            $"Link chỉ có hiệu lực trong 5 phút", isHtml: true)
            .SendAsync();

        await _context.EmailVerificationTokens.AddAsync(emailVerificationToken);

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();   
    }
}
