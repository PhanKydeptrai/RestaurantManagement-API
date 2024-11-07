using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.VoucherFeature.Commands.DeleteVoucher;

public class DeleteVoucherCommandValidator : AbstractValidator<DeleteVoucherCommand>
{
    public DeleteVoucherCommandValidator(IVoucherRepository voucherRepository)
    {
        RuleFor(p => p.id)
            .Must(a => voucherRepository.IsVoucherIdExists(Ulid.Parse(a)).Result == true)
            .WithMessage("Voucher not found.")
            .When(a => Ulid.TryParse(a.id, out _) == true)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("{PropertyName} is not a valid Ulid.");
    }
}
