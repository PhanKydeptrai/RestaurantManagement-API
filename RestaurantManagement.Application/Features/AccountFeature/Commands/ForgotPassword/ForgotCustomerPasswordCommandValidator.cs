using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ForgotPassword;

public class ForgotCustomerPasswordCommandValidator : AbstractValidator<ForgotCustomerPasswordCommand>
{
    public ForgotCustomerPasswordCommandValidator(ICustomerRepository customerRepository)
    {
        RuleFor(p => p.email)
            .NotNull().WithMessage("{PropertyName} is required.")
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .EmailAddress().WithMessage("{PropertyName} is not a valid email address.")
            .Must(a => customerRepository.IsCustomerEmailExist(a).Result)
            .WithMessage("Email is not Exist");

    }
}
