using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.RestoreSellStatus;

public class RestoreSellStatusCommandValidator : AbstractValidator<RestoreSellStatusCommand>
{
    public RestoreSellStatusCommandValidator(IMealRepository mealRepository)
    {
        RuleFor(a => a.id)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => mealRepository.GetSellStatus(a).Result == "InActive")
            .WithMessage("Meal is Active.");
    }
}
