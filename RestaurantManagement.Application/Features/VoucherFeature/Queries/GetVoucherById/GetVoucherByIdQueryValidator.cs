using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.VoucherFeature.Queries.GetVoucherById;

public class GetVoucherByIdQueryValidator : AbstractValidator<GetVoucherByIdQuery>
{
    public GetVoucherByIdQueryValidator(IVoucherRepository voucherRepository)
    {
        RuleFor(a => a.id)
            .NotNull()
            .WithMessage("Voucher Id is required")
            .NotEmpty()
            .WithMessage("Voucher Id is required")
            .Must(a => voucherRepository.IsVoucherIdExists(a).Result == true)
            .WithMessage("Voucher not found");
    }
}
