using FluentValidation;

namespace RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetTableTypeHistory;

public class GetTableTypeHistoryQueryValidator : AbstractValidator<GetTableTypeHistoryQuery>
{
    public GetTableTypeHistoryQueryValidator()
    {
        RuleFor(a => a.filterUserId)
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("Invalid User Id")
            .When(a => !string.IsNullOrEmpty(a.filterUserId));
    }
}
