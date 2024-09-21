using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.UpdateCategory;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    
    public UpdateCategoryValidator(ICategoryRepository _categoryRepository)
    {
        
        RuleFor(p => p.CategoryName)
            .NotEmpty()
            .WithMessage("Category name is required.")
            .MaximumLength(50)
            .WithMessage("Category name must not exceed 50 characters.")
            .Must(p  => _categoryRepository.IsCategoryNameExistsWhenUpdate(p).Result == false);
        RuleFor(p => p.CategoryStatus)
            .NotEmpty()
            .WithMessage("Category status is required.")
            .Must(p => p == "kd" || p == "nkd");
    }

}
