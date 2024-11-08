using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.RestoreMeal;

public class RestoreMealCommandValidator : AbstractValidator<RestoreMealCommand>
{
    public RestoreMealCommandValidator(IMealRepository mealRepository)
    {
        RuleFor(a => a.id)
            .Must(a => mealRepository.IsMealExist(Ulid.Parse(a)).Result == true)
            .WithMessage("Meal is not found")
            .Must(a => mealRepository.GetMealStatus(Ulid.Parse(a)).Result == "InActive")
            .WithMessage("Meal status is Active")
            .When(a => Ulid.TryParse(a.id, out _))
            .NotEmpty()
            .WithMessage("MealId is required")
            .NotNull()
            .WithMessage("MealId is required")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("MealId is invalid");

    }
}
