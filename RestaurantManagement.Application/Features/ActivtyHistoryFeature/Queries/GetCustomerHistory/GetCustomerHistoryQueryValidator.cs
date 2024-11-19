using FluentValidation;

namespace RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetCustomerHistory;

public class GetCustomerHistoryQueryValidator : AbstractValidator<GetCustomerHistoryQuery>
{
    public GetCustomerHistoryQueryValidator()
    {
        RuleFor(a => a.filterUserId)
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("Invalid User Id")
            .When(a => !string.IsNullOrEmpty(a.filterUserId));
    }
}
