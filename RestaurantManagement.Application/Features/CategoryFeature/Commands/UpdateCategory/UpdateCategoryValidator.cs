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
            .Must((id, name) => _categoryRepository.IsCategoryNameExistsWhenUpdate(id.CategoryId, name).Result == false)
            .WithMessage("Category name already exists.");

    }

}
