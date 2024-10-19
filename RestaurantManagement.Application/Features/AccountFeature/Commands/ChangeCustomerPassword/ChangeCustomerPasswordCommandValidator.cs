using FluentValidation;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ChangeCustomerPassword;

public class ChangeCustomerPasswordCommandValidator : AbstractValidator<ChangeCustomerPasswordCommand>
{
    public ChangeCustomerPasswordCommandValidator()
    {
        RuleFor(a => a.newPass)
            .NotEqual(a => a.oldPass)
            .WithMessage("New password must be different from the old password.")
            .NotNull()
            .WithMessage("New password is required.")
            .NotEmpty()
            .WithMessage("New password is required.")
            .MinimumLength(8)
            .WithMessage("Old password must be at least 8 characters long.");

        RuleFor(a => a.oldPass)
            .NotNull()
            .WithMessage("Old password is required.")
            .NotEmpty()
            .WithMessage("Old password is required.")
            .MinimumLength(8)
            .WithMessage("Old password must be at least 8 characters long.");

    }
}
