using FluentValidation;

namespace RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetVoucherHistory;

public class GetVoucherHistoryQueryValidator : AbstractValidator<GetVoucherHistoryQuery>
{ 
    public GetVoucherHistoryQueryValidator()
    {
         RuleFor(a => a.filterUserId)
			.Must(a => Ulid.TryParse(a, out _))
			.WithMessage("Invalid User Id")
			.When(a => !string.IsNullOrEmpty(a.filterUserId));
    }
}
