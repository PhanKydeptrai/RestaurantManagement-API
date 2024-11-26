using FluentValidation;

namespace RestaurantManagement.Application.Features.OrderFeature.Queries.GetMakePaymentInformation;

public class GetMakePaymentInformationQueryValidator : AbstractValidator<GetMakePaymentInformationQuery>
{
    public GetMakePaymentInformationQueryValidator()
    {
        RuleFor(a => a.tableId)
            .NotNull()
            .WithMessage("TableId is required")
            .NotEmpty()
            .WithMessage("TableId is required")
            .Must(a => int.TryParse(a, out _))
            .WithMessage("TableId must be a number");
    }
}
