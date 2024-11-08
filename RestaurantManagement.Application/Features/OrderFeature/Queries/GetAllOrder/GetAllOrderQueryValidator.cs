using FluentValidation;

namespace RestaurantManagement.Application.Features.OrderFeature.Queries.GetAllOrder;

public class GetAllOrderQueryValidator : AbstractValidator<GetAllOrderQuery>
{
    public GetAllOrderQueryValidator()
    {
        RuleFor(a => a.filterUserId)
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("filterUserId is not valid")
            .When(a => !string.IsNullOrEmpty(a.filterUserId));

        RuleFor(a => a.filterTableId)
            .Must(a => int.TryParse(a, out _))
            .WithMessage("filterTableId is not valid")
            .When(a => !string.IsNullOrEmpty(a.filterTableId));

    }
}
