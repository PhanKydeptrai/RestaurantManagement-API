using FluentValidation;

namespace RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetCategoryHistory;

public class GetCategoryHistoryQueryValidator : AbstractValidator<GetCategoryHistoryQuery>
{
    public GetCategoryHistoryQueryValidator()
    {
        RuleFor(a => a.filterUserId)
			.Must(a => Ulid.TryParse(a, out _))
			.WithMessage("Invalid User Id")
			.When(a => !string.IsNullOrEmpty(a.filterUserId));
    }
}
