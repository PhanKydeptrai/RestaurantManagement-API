using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ForgotEmployeePassword;

public class ForgotEmployeePasswordCommandHandler : ICommandHandler<ForgotEmployeePasswordCommand>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFluentEmail _fluentEmail;
    private readonly IEmailVerify _emailVerify;
    private readonly IApplicationDbContext _context;
    public ForgotEmployeePasswordCommandHandler(
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork,
        IFluentEmail fluentEmail,
        IEmailVerify emailVerify,
        IApplicationDbContext context)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _fluentEmail = fluentEmail;
        _emailVerify = emailVerify;
        _context = context;
    }

    public async Task<Result> Handle(ForgotEmployeePasswordCommand request, CancellationToken cancellationToken)
    {
        
        var validator = new ForgotEmployeePasswordCommandValidator(_employeeRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        //Get user by email
        Ulid userId = await _context.Employees
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
