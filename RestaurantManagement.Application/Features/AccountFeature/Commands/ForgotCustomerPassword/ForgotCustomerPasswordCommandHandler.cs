﻿using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ForgotCustomerPassword;

internal class ForgotCustomerPasswordCommandHandler(
    ICustomerRepository customerRepository,
    IApplicationDbContext context,
    IUnitOfWork unitOfWork,
    IFluentEmail fluentEmail,
    IEmailVerify emailVerify) : ICommandHandler<ForgotCustomerPasswordCommand>
{
    public async Task<Result> Handle(ForgotCustomerPasswordCommand request, CancellationToken cancellationToken)
    {
        var validator = new ForgotCustomerPasswordCommandValidator(customerRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }


        //Get user by email
        Ulid userId = await context.Customers
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
        await fluentEmail.To(request.email).Subject("Mail xác nhận")
            .Body($"Vui lòng nhấn vào link sau để nhận mật khẩu mới: <a href='{verificationLink}'>Click me</a>" +
            $"Link chỉ có hiệu lực trong 5 phút", isHtml: true)
            .SendAsync();

        await context.EmailVerificationTokens.AddAsync(emailVerificationToken);

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
