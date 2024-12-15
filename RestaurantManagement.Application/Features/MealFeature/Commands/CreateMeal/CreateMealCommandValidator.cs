using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.CreateMeal;
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
            .NotNull().WithMessage("{PropertyName} is required.")
            .NotNull().WithMessage("{PropertyName} is required.")
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .Must(p => decimal.TryParse(p.ToString(), out _))
            .WithMessage("{PropertyName} must be a decimal.")
            .GreaterThan(0)
            .WithMessage("{PropertyName} must be greater than 0");

        RuleFor(p => p.CategoryId)
            .Must(p => categoryRepository.CheckStatusOfCategory(Ulid.Parse(p)).Result)
            .WithMessage("{PropertyName} is not valid")
            .When(a => Ulid.TryParse(a.CategoryId, out _))
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("{PropertyName} is not valid");



    }
}
