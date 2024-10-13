using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ForgotCustomerPassword;

public class ForgotCustomerPasswordCommandValidator : AbstractValidator<ForgotCustomerPasswordCommand>
{
    public ForgotCustomerPasswordCommandValidator(ICustomerRepository customerRepository)
    {
        RuleFor(p => p.email)
            .NotNull().WithMessage("{PropertyName} is required.")
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .EmailAddress().WithMessage("{PropertyName} is not a valid email address.")
            .Must(a => customerRepository.IsCustomerAccountActive(a).Result)
            .WithMessage("Email is not Exist");

    }
}
