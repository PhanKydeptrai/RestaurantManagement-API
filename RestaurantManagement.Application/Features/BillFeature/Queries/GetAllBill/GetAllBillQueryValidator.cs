using FluentValidation;

namespace RestaurantManagement.Application.Features.BillFeature.Queries.GetAllBill;

public class GetAllBillQueryValidator : AbstractValidator<GetAllBillQuery>
{
    public GetAllBillQueryValidator()
    {
        RuleFor(a => a.filter)
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("Invalid User Id")
            .When(a => !string.IsNullOrEmpty(a.filter));
    }
}
