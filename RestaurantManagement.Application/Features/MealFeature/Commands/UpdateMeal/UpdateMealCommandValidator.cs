using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.UpdateMeal;

public class UpdateMealCommandValidator : AbstractValidator<UpdateMealCommand>
{
    public UpdateMealCommandValidator(
        IMealRepository mealRepository,
        ICategoryRepository categoryRepository)
    {
        RuleFor(p => p.MealName)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(50)
            .WithMessage("{PropertyName} must not exceed 50 characters.")
            .Must((id, name) => mealRepository.IsMealNameUnique_update(Ulid.Parse(id.MealId), name).Result == false)
            .WithMessage("{PropertyName} is exist");

        RuleFor(p => p.MealId)
            .Must(a => mealRepository.IsMealExist(Ulid.Parse(a)).Result == true)
            .WithMessage("Meal is not found")
            .When(a => Ulid.TryParse(a.MealId, out _))
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .Must(p => Ulid.TryParse(p, out _))
            .WithMessage("{PropertyName} is not valid");

        RuleFor(p => p.Price)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull().WithMessage("{PropertyName} is required.")
            .Must(p => decimal.TryParse(p.ToString(), out _))
            .WithMessage("{PropertyName} must be a decimal.");
            

        RuleFor(p => p.CategoryId)
            .Must(p => categoryRepository.CheckStatusOfCategory(Ulid.Parse(p)).Result)
            .WithMessage("{PropertyName} is not valid")
            .When(a => Ulid.TryParse(a.CategoryId, out _))
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .Must(p => Ulid.TryParse(p, out _))
            .WithMessage("{PropertyName} is not valid");


    }
}
