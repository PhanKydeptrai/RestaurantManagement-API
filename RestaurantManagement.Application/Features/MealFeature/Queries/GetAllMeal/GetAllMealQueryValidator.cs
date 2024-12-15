using FluentValidation;

namespace RestaurantManagement.Application.Features.MealFeature.Queries.GetAllMeal;

public class GetAllMealQueryValidator : AbstractValidator<GetAllMealQuery>
{
    public GetAllMealQueryValidator()
    {
        // RuleFor(a => a.filterCategory)
        //     .Must(a => Ulid.TryParse(a, out _))
        //     .WithMessage("filterCategory is not valid")
        //     .When(a => !string.IsNullOrEmpty(a.filterCategory));

    }

}
