using System;
using FluentValidation;

namespace RestaurantManagement.Application.Features.ActivtyHistoryFeature.Queries.GetEmployeeHistory;

public class GetEmployeeHistoryQueryValidator : AbstractValidator<GetEmployeeHistoryQuery>
{
    public GetEmployeeHistoryQueryValidator()
    {
        RuleFor(a => a.filterUserId)
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("Invalid User Id")
            .When(a => !string.IsNullOrEmpty(a.filterUserId));
    }
}
