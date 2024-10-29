using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.RestoreMeal;

public class RestoreMealCommandValidator : AbstractValidator<RestoreMealCommand>
{
    public RestoreMealCommandValidator(IMealRepository mealRepository)
    {
        RuleFor(a => a.id)
            .NotEmpty()
            .WithMessage("MealId is required")
            .NotNull()
            .WithMessage("MealId is required")
            .Must(a => mealRepository.IsMealExist(a).Result == true)
            .WithMessage("Meal is not found")
            .Must(a => mealRepository.GetMealStatus(a).Result == "InActive")
            .WithMessage("Meal status is Active");

    }
}
