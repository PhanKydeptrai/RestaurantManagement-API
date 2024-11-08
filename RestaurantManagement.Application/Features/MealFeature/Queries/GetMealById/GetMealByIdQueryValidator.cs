using FluentValidation;

namespace RestaurantManagement.Application.Features.MealFeature.Queries.GetMealById;

public class GetMealByIdQueryValidator : AbstractValidator<GetMealByIdQuery>
{
    public GetMealByIdQueryValidator()
    {
        RuleFor(a => a.id)
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("filterUserId is not valid")
            .When(a => !string.IsNullOrEmpty(a.id));
    }
}
