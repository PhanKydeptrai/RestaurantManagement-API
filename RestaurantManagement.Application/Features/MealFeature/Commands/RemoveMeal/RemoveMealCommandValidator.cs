using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.RemoveMeal;

public class RemoveMealCommandValidator : AbstractValidator<RemoveMealCommand>
{
    public RemoveMealCommandValidator(IMealRepository mealRepository)
    {
        RuleFor(a => a.id)
            .Must(a => mealRepository.GetMealStatus(Ulid.Parse(a)).Result == "Active")
            .WithMessage("Meal is InActive")
            .When(a => Ulid.TryParse(a.id, out _))
            .NotEmpty()
            .WithMessage("Id is required")
            .NotNull()
            .WithMessage("Id is required")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("Id is invalid");
    }
}
