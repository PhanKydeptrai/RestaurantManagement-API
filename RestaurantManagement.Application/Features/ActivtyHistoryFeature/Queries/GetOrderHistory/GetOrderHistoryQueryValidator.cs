using FluentValidation;

namespace RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetOrderHistory;

public class GetOrderHistoryQueryValidator : AbstractValidator<GetOrderHistoryQuery>
{
    public GetOrderHistoryQueryValidator()
    {
        RuleFor(a => a.filterUserId)
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("Invalid User Id")
            .When(a => !string.IsNullOrEmpty(a.filterUserId));
    }
}
