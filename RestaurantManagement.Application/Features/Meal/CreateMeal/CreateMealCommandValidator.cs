using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.Meal.CreateMeal;

public class CreateMealCommandValidator : AbstractValidator<CreateMealCommand>
{
    
    public CreateMealCommandValidator(IMealRepository _mealRepository)
    {
        RuleFor(p => p.MealName).NotEmpty()
                                .WithMessage("Meal name is required")
                                .NotNull()
                                .WithMessage("Meal name is required")
                                .Must(p => _mealRepository.IsMealNameUnique(p).Result == true);
                                
        RuleFor(p => p.Price).NotEmpty()
                             .WithMessage("Price is required")
                             .NotNull()
                             .WithMessage("Price is required")
                             .Must(p => decimal.TryParse(p.ToString(), out _))
                             .WithMessage("Price must be a number");

        RuleFor(p => p.CategoryId).NotEmpty()
                                  .WithMessage("Category is required")
                                  .NotNull().WithMessage("Category is required");
    }
    
}
