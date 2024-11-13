using FluentValidation;

namespace RestaurantManagement.Application.Features.BillFeature.Queries.GetBillById;

public class GetBillByIdQueryValidator : AbstractValidator<GetBillByIdQuery>
{
    public GetBillByIdQueryValidator()
    {
        RuleFor(x => x.billId)
            .NotNull().WithMessage("{PropertyName} is required.")
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("{PropertyName} is not valid.");

    }
}
