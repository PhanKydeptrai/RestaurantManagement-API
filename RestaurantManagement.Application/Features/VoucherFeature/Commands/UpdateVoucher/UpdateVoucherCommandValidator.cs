using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.VoucherFeature.Commands.UpdateVoucher;

public class UpdateVoucherCommandValidator : AbstractValidator<UpdateVoucherCommand>
{
    public UpdateVoucherCommandValidator(IVoucherRepository voucherRepository)
    {
        RuleFor(a => a.VoucherId)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => voucherRepository.IsVoucherIdExists(a).Result == true)
            .WithMessage("VoucherId does not exists.");
        
        RuleFor(a => a.VoucherName)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(50)
            .WithMessage("{PropertyName} must not exceed 50 characters")
            .Must((a, name) => voucherRepository.IsVoucherNameExists(name, a.VoucherId).Result == false)
            .WithMessage("Name already exists.");

        RuleFor(a => a.MaxDiscount)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(p => decimal.TryParse(p.ToString(), out _))
            .WithMessage("{PropertyName} must be a decimal.");

        RuleFor(a => a.VoucherCondition)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(p => decimal.TryParse(p.ToString(), out _))
            .WithMessage("{PropertyName} must be a decimal.");

        RuleFor(a => a.StartDate)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(p => p > DateTime.UtcNow)
            .WithMessage("{PropertyName} must be greater than current date.");

        RuleFor(a => a.ExpiredDate)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must((a, b) => b > a.StartDate)
            .WithMessage("{PropertyName} must be greater than StartDate.");
    }
}
