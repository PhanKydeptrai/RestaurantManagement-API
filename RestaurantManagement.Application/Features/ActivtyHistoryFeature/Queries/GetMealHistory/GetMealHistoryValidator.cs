using FluentValidation;

namespace RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetMealHistory;

public class GetMealHistoryValidator : AbstractValidator<GetMealHistoryQuery>
{
    public GetMealHistoryValidator()
    {
        RuleFor(a => a.filterUserId)
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("Invalid User Id")
            .When(a => !string.IsNullOrEmpty(a.filterUserId));
    }
}
