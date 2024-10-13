using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ForgotEmployeePassword;

public class ForgotEmployeePasswordCommandValidator : AbstractValidator<ForgotEmployeePasswordCommand>
{
    public ForgotEmployeePasswordCommandValidator(IEmployeeRepository employeeRepository)
    {
        RuleFor(p => p.email)
            .NotNull().WithMessage("{PropertyName} is required.")
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .EmailAddress().WithMessage("{PropertyName} is not a valid email address.")
            .Must(a => employeeRepository.IsEmployyeEmailExist(a).Result)
            .WithMessage("Email is not Exist");
    }
}
