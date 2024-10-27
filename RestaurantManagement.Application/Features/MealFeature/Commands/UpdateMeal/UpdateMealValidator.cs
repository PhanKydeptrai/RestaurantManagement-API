using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.UpdateMeal;

public class UpdateMealValidator : AbstractValidator<UpdateMealCommand>
{
    public UpdateMealValidator(
        IMealRepository mealRepository,
        ICategoryRepository categoryRepository)
    {
        RuleFor(p => p.MealName)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull().WithMessage("{PropertyName} is required.")
            .MaximumLength(50)
            .WithMessage("{PropertyName} must not exceed 50 characters.")
            .Must((id, name) => mealRepository.IsMealNameUnique_update(id.MealId, name).Result == false)
            .WithMessage("{PropertyName} is exist");

        RuleFor(p => p.MealId)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull().WithMessage("{PropertyName} is required.")
            .Must(a => mealRepository.IsMealExist(a).Result == true)
            .WithMessage("Meal is not found");

        RuleFor(p => p.Price)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull().WithMessage("{PropertyName} is required.")
            .Must(p => decimal.TryParse(p.ToString(), out _))
            .WithMessage("{PropertyName} must be a decimal.");
            

        RuleFor(p => p.CategoryId)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull().WithMessage("{PropertyName} is required.")
            .Must(p => categoryRepository.CheckStatusOfCategory(p).Result)
            .WithMessage("{PropertyName} is not valid");


    }
}
