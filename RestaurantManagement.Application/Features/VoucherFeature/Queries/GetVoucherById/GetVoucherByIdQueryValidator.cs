using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.VoucherFeature.Queries.GetVoucherById;

public class GetVoucherByIdQueryValidator : AbstractValidator<GetVoucherByIdQuery>
{
    public GetVoucherByIdQueryValidator(IVoucherRepository voucherRepository)
    {
        RuleFor(a => a.id)
            .Must(a => voucherRepository.IsVoucherIdExists(Ulid.Parse(a)).Result == true)
            .WithMessage("Voucher not found")
            .When(a => Ulid.TryParse(a.id, out _))
            .NotNull()
            .WithMessage("Voucher Id is required")
            .NotEmpty()
            .WithMessage("Voucher Id is required")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("Voucher Id is invalid");
    }
}
