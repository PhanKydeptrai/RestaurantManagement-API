using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ForgotEmployeePassword;

public class ForgotEmployeePasswordCommandHandler(
    IEmployeeRepository employeeRepository,
    IUnitOfWork unitOfWork,
    IFluentEmail fluentEmail,
    IEmailVerify emailVerify,
    IApplicationDbContext context) : ICommandHandler<ForgotEmployeePasswordCommand>
{
    public async Task<Result> Handle(ForgotEmployeePasswordCommand request, CancellationToken cancellationToken)
    {
        //Validate request
        var validator = new ForgotEmployeePasswordCommandValidator(employeeRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        //Get user by email
        Ulid userId = await context.Employees
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
        var verificationLink = emailVerify.CreateLinkForResetPass(emailVerificationToken);

        bool emailSent = false;
        int retryCount = 0;
        int maxRetries = 5;

        do
        {
            try
            {
                await fluentEmail.To(request.email).Subject("Nhà hàng Nhum nhum - Nhận mật khẩu mới")
                        .Body($"Vui lòng nhấn vào link sau để nhận mật khẩu mới: <a href='{verificationLink}'>Click me</a>" +
                        $"Link chỉ có hiệu lực trong 5 phút", isHtml: true)
                        .SendAsync();
                emailSent = true;
            }
            catch
            {
                retryCount++;
                if (retryCount >= maxRetries)
                {
                    return Result.Failure(new[] { new Error("Email", "Failed to send email") });
                }
            }
        }
        while (!emailSent && retryCount < maxRetries);


        await context.EmailVerificationTokens.AddAsync(emailVerificationToken);

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
