using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.ChangeSellStatus;

public class ChangeSellStatusCommandValidator : AbstractValidator<ChangeSellStatusCommand>
{
    public ChangeSellStatusCommandValidator(IMealRepository mealRepository)
    {
        RuleFor(a => a.id)
            .Must(a => mealRepository.GetSellStatus(Ulid.Parse(a)).Result == "Active")
            .WithMessage("Meal is InActive.")
            .When(a => Ulid.TryParse(a.id, out _))
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("{PropertyName} is invalid.");
    }
}
