using FluentValidation;

namespace RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetTableHistory;

public class GetTableHistoryQueryValidator : AbstractValidator<GetTableHistoryQuery>
{
    public GetTableHistoryQueryValidator()
    {
        RuleFor(a => a.filterUserId)
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("Invalid User Id")
            .When(a => !string.IsNullOrEmpty(a.filterUserId));
    }
}
