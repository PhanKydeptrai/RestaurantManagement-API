using FluentValidation;

namespace RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetBillHistory;

public class GetBillHistoryQueryValidator : AbstractValidator<GetBillHistoryQuery>
{
    public GetBillHistoryQueryValidator()
    {
            RuleFor(a => a.filterUserId)
                .Must(a => Ulid.TryParse(a, out _))
                .WithMessage("Invalid User Id")
                .When(a => !string.IsNullOrEmpty(a.filterUserId));
    }
}
