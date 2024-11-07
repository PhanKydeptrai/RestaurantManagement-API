using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.VoucherFeature.Commands.UpdateVoucher;

public class UpdateVoucherCommandValidator : AbstractValidator<UpdateVoucherCommand>
{
    public UpdateVoucherCommandValidator(IVoucherRepository voucherRepository)
    {
        RuleFor(a => a.VoucherId)
            .Must(a => voucherRepository.IsVoucherIdExists(Ulid.Parse(a)).Result == true)
            .WithMessage("VoucherId does not exists.")
            .When(a => Ulid.TryParse(a.VoucherId, out _) == true)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("{PropertyName} must be a valid Ulid.");
        
        RuleFor(a => a.VoucherName)
            .MaximumLength(50)
            .WithMessage("{PropertyName} must not exceed 50 characters")
            .Must((a, name) => voucherRepository.IsVoucherNameExists(name, Ulid.Parse(a.VoucherId)).Result == false)
            .When(a => Ulid.TryParse(a.VoucherId, out _) == true)
            .WithMessage("Name already exists.")
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.");

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
