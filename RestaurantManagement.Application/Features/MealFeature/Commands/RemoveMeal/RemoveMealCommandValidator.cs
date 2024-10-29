using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.RemoveMeal;

public class RemoveMealCommandValidator : AbstractValidator<RemoveMealCommand>
{
    public RemoveMealCommandValidator(IMealRepository mealRepository)
    {
        RuleFor(a => a.id)
            .NotEmpty()
            .WithMessage("Id is required")
            .NotNull()
            .WithMessage("Id is required")
            .Must(a => mealRepository.GetMealStatus(a).Result == "InActive")
            .WithMessage("Meal is InActive");
    }
}
