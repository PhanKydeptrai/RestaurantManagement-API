using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.MealFeature.Commands;
public class CreateMealCommandValidator : AbstractValidator<CreateMealCommand>
{
    public CreateMealCommandValidator(
        IMealRepository mealRepository,
        ICategoryRepository categoryRepository)
    {
        RuleFor(p => p.MealName)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull().WithMessage("{PropertyName} is required.")
            .MaximumLength(50)
            .WithMessage("{PropertyName} must not exceed 50 characters.")
            .Must(p => mealRepository.IsMealNameUnique(p).Result == false)
            .WithMessage("{PropertyName} is exist");

        RuleFor(p => p.Price)
            .Must(p => p is decimal)
            .WithMessage("{PropertyName} must be a decimal.")
            .When(p => p != null).WithMessage("{PropertyName} must be a decimal.")
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull().WithMessage("{PropertyName} is required.");
        
        RuleFor(p => p.CategoryId)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull().WithMessage("{PropertyName} is required.")
            .Must(p => categoryRepository.CheckStatusOfCategory(p).Result == false)
            .WithMessage("{PropertyName} is not valid");



    }
}
