using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.ChangeSellStatus;

public class ChangeSellStatusCommandValidator : AbstractValidator<ChangeSellStatusCommand>
{
    public ChangeSellStatusCommandValidator(IMealRepository mealRepository)
    {
        RuleFor(a => a.id)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => mealRepository.GetSellStatus(a).Result == "kd")
            .WithMessage("Meal is nkd.");
    }
}
